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
	// Parse the sample result from the My Bookings GET request

	var fname = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "my-bookings-web-page.html");
	var html = File.ReadAllText(fname);

	var parser = new HtmlParser();
	var doc = await parser.ParseAsync(html);

	var bookingform = doc.All
		.Where(x => x.LocalName == "div" && x.ClassList.Contains("formInput"))
		//.Where(x => x.Children.HasClass("fieldSetWrapper"))
        .Children("div.fieldSetWrapper")
        .Children("div.fieldSetContent")
        .Children("fieldset")
        ;
    //bookingform.Dump();

    // find the <legend> with "My Bookings" content
    // Get the next <div> and then its children
    var bookingtitle = bookingform
        .Children("legend")
        .Where(x => x.TextContent == "My Bookings")
        .FirstOrDefault();
    var bookings = bookingtitle.NextElementSibling.Children;

    // Each booking consists of an H5 containing the activity name and centre followed by a <P> 
    // containing the instructor, <br/>, location, <br/>, date, time and a cancel link if applicable
    var result = new BookedItems();
    var current = bookings.FirstOrDefault();
    bool first = true;
    while (current != null)
    {
        if ((current is IHtmlHeadingElement) && (current.LocalName.ToLower() == "h5"))
        {
            var item = result.AddNew(current.TextContent);
            // Process the next <P> element
            current = current.NextElementSibling;
            if (current is IHtmlParagraphElement)
            {
                item.ParseBookingDetails(current.InnerHtml);
//                if (first)
//                {
//                    current.Dump();
//                    first = false;
//                }
            }
        }
        current = current.NextElementSibling;
    }
    
    result.Dump();
}

public class BookedItems
{
    private List<BookedItem> _items = new List<BookedItem>();

    public IEnumerable<BookedItem> Bookings { get { return _items; } }
    
    public BookedItem AddNew(string activityAndCentre)
    {
        var result = new BookedItem();
        _items.Add(result);
        var match = ActivityAndCentreRE.Match(activityAndCentre);
        if (match.Success)
        {
            result.Activity = match.Groups["activity"].Value.Trim();
            result.Centre = match.Groups["centre"].Value.Trim();
        }
        return result;
    }
}

public static string ActivityAndCentrePattern = @"(?<activity>[\w\s]*)\s+\((?<centre>[\w\s]*)\)";
public static Regex ActivityAndCentreRE = new Regex(ActivityAndCentrePattern);
public static string BookingDetailPattern = @"Instructor:\s*(?<instructor>[\w\s]*)\s+\((?<status>[\w\s]*)\)\<br[\s\/]*\>Location:\s+(?<location>[\w\d\s]*)\<br[\s\/]*\>(?<datetime>.*)";
public static Regex BookingDetailRE = new Regex(BookingDetailPattern);
public static string BookingDateTimePattern = @"Date: (?<fullDate>(?<day>\d{1,2})\s+(?<month>\w*)\s+(?<year>\d{4}))\s+(?<fromHour>\d{1,2}):(?<fromMinute>\d{1,2})\s+-\s+(?<toHour>\d{1,2}):(?<toMinute>\d{1,2})([\s-]{0,3}\<a href=.*bookingid=(?<bookingId>\d*).*\</a\>)*";
public static Regex BookingDateTimeRE = new Regex(BookingDateTimePattern);

public class BookedItem
{
    public int Id { get; set; }
    public string Activity { get; set; }
    public string Centre { get; set; }
    public string Instructor { get; set; }
    public string Location { get; set; }
    public string Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    
    public void ParseBookingDetails(string source)
    {
        var match = BookingDetailRE.Match(source);
        if (match.Success)
        {
            Instructor = match.Groups["instructor"].Value.Trim();
            Status = match.Groups["status"].Value.Trim();
            Location = match.Groups["location"].Value.Trim();
            
            ParseDateTime(match.Groups["datetime"].Value);
        }
    }
    
    private void ParseDateTime(string source)
    {
        var match = BookingDateTimeRE.Match(source);
        if (match.Success)
        {
            int fromHour = int.Parse(match.Groups["fromHour"].Value);
            int fromMin = int.Parse(match.Groups["fromMinute"].Value);
            int toHour = int.Parse(match.Groups["toHour"].Value);
            int toMin = int.Parse(match.Groups["toMinute"].Value);
            
            StartDate = DateTime.Parse(match.Groups["fullDate"].Value)
                .AddHours(fromHour)
                .AddMinutes(fromMin);
            EndDate = DateTime.Parse(match.Groups["fullDate"].Value)
                .AddHours(toHour)
                .AddMinutes(toMin);
            StartTime = new TimeSpan(fromHour, fromMin, 0);
            EndTime = new TimeSpan(toHour, toMin, 0);
            string idtxt = match.Groups["bookingId"].Success ? match.Groups["bookingId"].Value : "0";
            Id = int.Parse(idtxt);
        }
    }
}
