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

	var bookings = doc.All
		.Where(x => x.LocalName == "div" && x.ClassList.Contains("formInput"))
		.Where(x => x.Children.HasClass("fieldSetWrapper"));
		//.Where(x => x.ClassList.Contains("fieldSetContent"));


	bookings.Dump();
}