<Query Kind="Program">
  <NuGetReference>AngleSharp</NuGetReference>
  <Namespace>AngleSharp</Namespace>
  <Namespace>AngleSharp.Extensions</Namespace>
  <Namespace>AngleSharp.Parser.Html</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>AngleSharp.Dom.Html</Namespace>
</Query>

async Task Main()
{
    // Parse the sample result from the Timetable GET request
    
    var fname = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "timetable-fragment.html");
    var html = File.ReadAllText(fname);
        
    var parser = new HtmlParser();
    var doc = await parser.ParseAsync(html);
    
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
    
    var resultContainer = doc.All.Where(x => x.LocalName == "div" && x.Id == "resultContainer");
    //resultContainer.Dump();
    var activities = doc.All.Where(x => x.ClassList.Contains("activityHeader"));
    var ttelements = doc.All.Where(x => x.ClassList.Contains("resultTable"));
    //activities.Dump();
    //timetables.Dump();

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
            };
            tt.AddItem(tti);
        }
        //timetable.Dump();        
    }
    
    timetables.Dump();
}

public static string bookingLinkPattern = @"onclick=['\""]addBooking\((?<id>\d+)\)";
public static Regex bookingLinkRE = new Regex(bookingLinkPattern);

public int GetTimetableId(string cellText)
{
    int result = 0;
    var match = bookingLinkRE.Match(cellText);
    if (match.Success)
    {
        result = int.Parse(match.Groups["id"].Value);
    }
    return result;
}

public static string timePattern = @"(?<fromHour>\d+):(?<fromMinute>\d+)\s*-\s*(?<toHour>\d+):(?<toMinute>\d+)";
public static Regex timeRE = new Regex(timePattern);

public Tuple<TimeSpan, TimeSpan> GetStartEndTime(string cellText)
{
    var match = timeRE.Match(cellText);
    if (match.Success)
    {
        TimeSpan tFrom = new TimeSpan(int.Parse(match.Groups["fromHour"].Value), int.Parse(match.Groups["fromMinute"].Value), 0);
        TimeSpan tTo = new TimeSpan(int.Parse(match.Groups["toHour"].Value), int.Parse(match.Groups["toMinute"].Value), 0);
        return new Tuple<TimeSpan, TimeSpan>(tFrom, tTo);
    }
    return null;
}

public class Timetable
{
    private List<TimetableItem> _items = new List<TimetableItem>();
    
    public string Title { get; set; }
    public IEnumerable<TimetableItem> Items { get { return _items; } }
    
    public void AddItem(TimetableItem item)
    {
        _items.Add(item);
    }
}

public class TimetableItem
{
    public int Id { get; set; } = 0;
    public string Centre { get; set; }
    public string Day { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string Availability { get; set; }
    public string Instructor { get; set; }    
}
