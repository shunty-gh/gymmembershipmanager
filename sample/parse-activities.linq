<Query Kind="Program" />

void Main()
{
    // Parse the sample activities GET result
    
    var fname = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "activities-fragment.html");
    var html = File.ReadAllText(fname);
    var el = XElement.Parse("<activities>" + html + "</activities>");
    
    var labels = el.Elements("div")
        .Where(x => x.Attributes().Any(a => a.Name == "class" && a.Value.Contains("activityItem")))
        .Select(x => x.Elements("label").FirstOrDefault())
        .Select(x => new Activity
        {
            Id = int.Parse(x.Element("input").Attribute("value").Value),
            Label = x.Value.Trim()
        })        
        .Dump();
        
}

public class Activity
{
    public int Id { get; set;}
    public string Label { get; set; }
}
