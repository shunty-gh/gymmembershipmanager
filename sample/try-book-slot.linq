<Query Kind="Program">
  <Reference Relative="..\GymMembershipManager\GymMembershipManager\bin\Debug\Shunty.GymMembershipManager.dll">D:\projects\sph\betterGym\GymMembershipManager\GymMembershipManager\bin\Debug\Shunty.GymMembershipManager.dll</Reference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <NuGetReference>Serilog</NuGetReference>
  <NuGetReference>Serilog.Enrichers.Thread</NuGetReference>
  <NuGetReference>Serilog.Sinks.LINQPad</NuGetReference>
  <NuGetReference>Serilog.Sinks.Literate</NuGetReference>
  <NuGetReference>Serilog.Sinks.Seq</NuGetReference>
  <Namespace>Serilog</Namespace>
  <Namespace>Shunty.GymMembershipManager</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

async Task Main()
{
    InitialiseLogging();

    var user = Util.GetPassword("BetterGym-User");
    var pwd = Util.GetPassword("BetterGym");

    using (var conn = ConnectionManager.CreateManager())
    {

        await conn.Login(user, pwd);

        var activityAction = new ActivitiesAction(conn);
        var activities = await activityAction.GetActivitiesAsync();
        //activities.Dump();

        var activityId = activities.First(a => a.Title.StartsWith("Group Cycle")).Id;
        var timetableAction = new TimetablesAction(conn);
        var timetables = await timetableAction.GetTimetablesAsync(new[] { activityId });
        timetables.Dump();
        
        var item = timetables.First().Items.Where(t => t.Date == new DateTime(2017, 2, 2) && t.StartTime == new TimeSpan(9, 0, 0)).FirstOrDefault();
        item.Dump();

        // Add item to the basket
        var addbooking = await conn.GetResultAsync(Consts.AddBookingUrl, new[] { new KeyValuePair<string, string>("booking", item.Id.ToString()) });
        log.Debug("Add booking result Json {AddBookingResultJson}", addbooking);
        
        var addresult = Newtonsoft.Json.JsonConvert.DeserializeObject<AddBookingResult>(addbooking);
        addresult.Dump();

        if (addresult.Success)
        {
            // GET the basket
            var basket = await conn.GetResultAsync(Consts.BasketUrl);
            log.Debug("Basket result {BasketResult}", basket);

            // Checkout
            var checkout = await conn.GetResultAsync(Consts.BasketPayUrl, null, new[] {
                new KeyValuePair<string, string>("Referer", Consts.BaseAddress + Consts.BasketUrl),
                new KeyValuePair<string, string>("Upgrade-Insecure-Requests", "1")
            });
            checkout = await conn.GetResultAsync(Consts.BasketPaymentConfirmedUrl, null, new[] {
                new KeyValuePair<string, string>("Referer", Consts.BaseAddress + Consts.BasketUrl),
                new KeyValuePair<string, string>("Upgrade-Insecure-Requests", "1")
            });
            log.Debug("Checkout result {CheckoutResult}", basket);
        }
    }
}

private static Serilog.ILogger log = null;

public void InitialiseLogging()
{
    var sconfig = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .Enrich.WithThreadId()
        .WriteTo.LiterateConsole()
        .WriteTo.Seq("http://localhost:5341");

    log = sconfig.CreateLogger()
        .ForContext("SourceContext", "GymMembershipManager: Get my bookings sample");
    Log.Logger = log;
}