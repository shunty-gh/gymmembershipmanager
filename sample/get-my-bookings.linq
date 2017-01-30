<Query Kind="Program">
  <Reference Relative="..\GymMembershipManager\GymMembershipManager\bin\Debug\Shunty.GymMembershipManager.dll">D:\projects\sph\betterGym\GymMembershipManager\GymMembershipManager\bin\Debug\Shunty.GymMembershipManager.dll</Reference>
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
        var action = new MyBookingsAction(conn);
        var bookings = await action.GetMyBookingsAsync();
        bookings.Dump();
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
        .ForContext("SourceContext", "GymMembershipManager: Get activities sample");
    Log.Logger = log;
}