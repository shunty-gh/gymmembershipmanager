using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shunty.Logging;
using AngleSharp.Parser.Html;
using AngleSharp.Dom.Html;

namespace Shunty.GymMembershipManager
{
    public class TimetablesAction : MembershipAction
    {
        private static readonly ILog _log = LogProvider.For<TimetablesAction>();

        public TimetablesAction(IConnectionManager manager) 
            : base(manager)
        {
        }

        public async Task<IEnumerable<Timetable>> GetTimetablesAsync(IEnumerable<int> activityIds)
        {
            // Need to select the activity first
            var content = new List<KeyValuePair<string, string>>();
            foreach (var activityId in activityIds)
            {
                content.Add(new KeyValuePair<string, string>("activity", activityId.ToString()));
            }
            var response = await Manager.PostResultAsync(Consts.ActivitySelectUrl, content);
            _log.TraceFormat("Activity select response: {ActivitySelectResponse}", response);

            // Now POST to the timetable url
            content = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("X-Requested-With", "XMLHttpRequest"),
            };
            response = await Manager.PostResultAsync(Consts.TimetableUrl, content);
            _log.TraceFormat("Timetable POST response: {TimetablePostResponse}", response);

            // Now GET the timetable url
            response = await Manager.GetResultAsync(Consts.TimetableUrl);
            _log.TraceFormat("Timetable GET response: {TimetableGetResponse}", response);

            var result = await ParseTimetableText(response);
            return result;
        }

        private async Task<IEnumerable<Timetable>> ParseTimetableText(string source)
        {
            /* The whole timetable is structured as 
                <body class="timetableBody">
                    <div id="resultUpdate">
                        ...
                    </div>    
                    <div id="resultContainer">
                        <div>
                            activity timetable...
                        </div>
                    </div>
                    ....
                </body>

               Each activity timetable is structured as:
                <div class="activityHeader">activity title...</div>
                <div class="activityTop" />
                <div class="activityTile">
                    <div class="tableWrapper">
                        <table class="resultTable">
                          activity rows...
                        </table>
                    </div>
                </div>
                <div class="activityBottom" />    

              Each activity row in the table has the form:
                <tr><td>centre</td><td>day</td><td>date</td><td>time</td><td>availability</td><td>price</td><td>instructor</td><td>Add to basket</td>
              eg
                <tr >
                    <td>Highgrove</td>
                    <td>Saturday</td>
                    <td>28/01/2017</td>
                    <td>10:00 - 10:45</td>
                    <td>Fully Booked</td>
                    <td>-</td>
                    <td>Claire</td>
                    <td>[ FULLY BOOKED ]</td>
                </tr>
                ...
                <tr >
                    <td>Highgrove</td>
                    <td>Thursday</td>
                    <td>02/02/2017</td>
                    <td>06:45 - 07:30</td>
                    <td>3 Spaces</td>
                    <td>
                        <a href='GetPrice?instanceId=13201988' rel='GetPrice?instanceId=13201988' id='price13201988' class='jTip100'>
                            <img src='/media/siteimages/moreInfoBlue.gif' alt='Click for More info' style='border:0px;' />
                        </a>
                    </td>
                    <td>Claire</td>
                    <td>
                        <a id='slot13201988' href='#' onclick='addBooking(13201988)' class='addLink'>[ Add to Basket ]</a>
                    </td>
                </tr>

            */

            var parser = new HtmlParser();
            var doc = await parser.ParseAsync(source);
            var ttelements = doc.All.Where(x => x.ClassList.Contains("resultTable"));

            var timetables = new List<Timetable>();

            foreach (var ttelement in ttelements)
            {
                var wrapper = ttelement?.ParentElement;
                var tile = wrapper?.Parent;
                var header = tile?.PreviousSibling?.PreviousSibling;

                var tt = new Timetable();
                timetables.Add(tt);
                tt.Title = header?.TextContent ?? "";

                var table = ttelement as IHtmlTableElement;
                foreach (var row in table.Rows.Skip(1))
                {
                    var cells = row.Cells.ToList();
                    var times = GetStartEndTime(cells[3].TextContent);

                    var tti = new TimetableItem
                    {
                        Centre = cells[0].TextContent,
                        Day = cells[1].TextContent,
                        Date = DateTime.Parse(cells[2].TextContent),
                        StartTime = times?.Item1 ?? new TimeSpan(0),
                        EndTime = times?.Item2 ?? new TimeSpan(0),
                        Availability = cells[4].TextContent,
                        // cell 5 is price
                        Instructor = cells[6].TextContent,
                        // cell 7 contains the booking link if there is an available slot
                        Id = GetTimetableId(cells[7].InnerHtml),
                        Enrolled = IsEnrolled(cells[7].TextContent),
                    };
                    tt.AddItem(tti);
                }
            }
            return timetables;
        }

        private int GetTimetableId(string cellText)
        {
            int result = 0;
            if (string.IsNullOrWhiteSpace(cellText) || (cellText == "[ FULLY BOOKED ]") || (cellText == "[ ENROLLED ]"))
            {
                return result;
            }

            var match = PatternMatchers.BookingLinkRE.Match(cellText);
            if (match.Success)
            {
                result = int.Parse(match.Groups["id"].Value);
            }
            else
            {
                _log.DebugFormat("Cannot find timetable id for text: {IdCellText}", cellText);
            }
            return result;
        }

        private bool IsEnrolled(string cellText)
        {
            return ((!string.IsNullOrWhiteSpace(cellText)) && (cellText == "[ ENROLLED ]"));
        }

        private Tuple<TimeSpan, TimeSpan> GetStartEndTime(string cellText)
        {
            var match = PatternMatchers.TimeRE.Match(cellText);
            if (match.Success)
            {
                TimeSpan tFrom = new TimeSpan(int.Parse(match.Groups["fromHour"].Value), int.Parse(match.Groups["fromMinute"].Value), 0);
                TimeSpan tTo = new TimeSpan(int.Parse(match.Groups["toHour"].Value), int.Parse(match.Groups["toMinute"].Value), 0);
                return new Tuple<TimeSpan, TimeSpan>(tFrom, tTo);
            }
            else
            {
                _log.DebugFormat("Cannot find start and end times for text: {ActivityTimeCellText}", cellText);
            }
            return null;
        }

    }
}
