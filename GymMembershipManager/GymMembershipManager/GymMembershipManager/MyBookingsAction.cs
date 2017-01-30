using System;
using System.Linq;
using System.Threading.Tasks;
using Shunty.Logging;
using AngleSharp.Parser.Html;
using AngleSharp.Dom.Html;
using AngleSharp.Extensions;

namespace Shunty.GymMembershipManager
{
    public class MyBookingsAction : MembershipAction
    {
        private static readonly ILog _log = LogProvider.For<MyBookingsAction>();

        public MyBookingsAction(IConnectionManager manager) 
            : base(manager)
        { }

        public async Task<BookedItems> GetMyBookingsAsync()
        {
            var response = await Manager.GetResultAsync(Consts.MyBookingsUrl);
            _log.TraceFormat("My bookings response: {MyBookings}", response);

            var result = await ParseMyBookingsText(response);
            return result;
        }

        private const string formInputClass = "formInput";
        private const string wrapperSelector = "div.fieldSetWrapper";
        private const string fieldsetContentSelector = "div.fieldSetContent";
        private const string fieldsetSelector = "fieldset";
        private const string divName = "div";
        private const string h5Name = "h5";
        private const string legendSelector = "legend";
        private const string bookingsLegend = "My Bookings";

        private async Task<BookedItems> ParseMyBookingsText(string source)
        {
            var parser = new HtmlParser();
            var doc = await parser.ParseAsync(source);

            var bookingform = doc.All
                .Where(x => x.LocalName == divName && x.ClassList.Contains(formInputClass))
                .Children(wrapperSelector)
                .Children(fieldsetContentSelector)
                .Children(fieldsetSelector);

            // Find the <legend> with "My Bookings" content
            // Get the next <div> and then its children
            var bookingtitle = bookingform
                .Children(legendSelector)
                .Where(x => x.TextContent == bookingsLegend)
                .FirstOrDefault();
            var bookings = bookingtitle.NextElementSibling.Children;

            // Each booking consists of an H5 containing the activity name and centre followed by a <P> 
            // containing the instructor, <br/>, location, <br/>, date, time and a cancel link if applicable
            var result = new BookedItems();
            var current = bookings.FirstOrDefault();
            while (current != null)
            {
                if ((current is IHtmlHeadingElement) && (current.LocalName.ToLower() == h5Name))
                {
                    _log.TraceFormat("Parsing booking heading: {BookingHeadingHTML}", current.InnerHtml);

                    var item = result.AddNew(current.TextContent);
                    // Process the next <P> element
                    current = current.NextElementSibling;
                    if (current is IHtmlParagraphElement)
                    {
                        _log.TraceFormat("Parsing booking detail: {BookingDetailHTML}", current.InnerHtml);
                        item.ParseBookingDetails(current.InnerHtml);
                    }
                }
                current = current.NextElementSibling;
            }

            return result;
        }
    }
}
