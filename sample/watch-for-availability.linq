<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\Microsoft.VisualBasic.dll</Reference>
  <Reference Relative="..\GymMembershipManager\GymMembershipManager\bin\Debug\Shunty.GymMembershipManager.dll">D:\projects\sph\betterGym\GymMembershipManager\GymMembershipManager\bin\Debug\Shunty.GymMembershipManager.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Windows.Forms.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Security.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Configuration.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\Accessibility.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Deployment.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Runtime.Serialization.Formatters.Soap.dll</Reference>
  <NuGetReference>Serilog</NuGetReference>
  <NuGetReference>Serilog.Enrichers.Thread</NuGetReference>
  <NuGetReference>Serilog.Sinks.LINQPad</NuGetReference>
  <NuGetReference>Serilog.Sinks.Literate</NuGetReference>
  <NuGetReference>Serilog.Sinks.Seq</NuGetReference>
  <Namespace>Serilog</Namespace>
  <Namespace>Shunty.GymMembershipManager</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Microsoft.VisualBasic</Namespace>
  <Namespace>System.Windows.Forms</Namespace>
</Query>

// *** Watch specific activity classes and notify 
//     if/when a space becomes available ***

// *** Changes these as required ***
private static string activityName = "Group Cycle";
private static string dayToWatch = "Friday";
private static TimeSpan startTime = new TimeSpan(6, 45, 0);
private static TimeSpan pollInterval = new TimeSpan(0, 2, 0);
// *** ========================= ***    

async Task Main()
{
    //InitialiseLogging();

    var result = false;
    var waitHandle = new AutoResetEvent(false);
    var msg = "Waiting..";
    var timer = new System.Timers.Timer(pollInterval.TotalMilliseconds);
    timer.Elapsed += async (sender, e) => {
        result = await CheckAvailability();
        if (!result)
            Console.Write(msg);
        waitHandle.Set();
    };
    timer.Start();

    // Call it straight away, before the timers first call
    result = await CheckAvailability();
    Console.Write(msg);

    while (!result)
    {
        Console.Write(".");
        waitHandle.WaitOne(10000);
    }
    timer.Stop();
    
    if (result)
    {
        Interaction.MsgBox("A class has become available", MsgBoxStyle.OkOnly, "Class Space");
    }
}

public async Task<bool> CheckAvailability()
{
    var user = Util.GetPassword("BetterGym-User");
    var pwd = Util.GetPassword("BetterGym");
    var result = false;

    Console.WriteLine("");
    $"Starting new check...".Dump();
    IEnumerable<Activity> activities;
    using (var conn = ConnectionManager.CreateManager())
    {
        await conn.Login(user, pwd);
        var activityAction = new ActivitiesAction(conn);
        activities = await activityAction.GetActivitiesAsync();

        var activityId = activities.First(a => a.Title.StartsWith(activityName)).Id;
        var timetableAction = new TimetablesAction(conn);
        var timetables = await timetableAction.GetTimetablesAsync(new[] { activityId });
        //timetables.Dump();

        var spinclasses = timetables.First().Items;
        var fridayclasses = spinclasses.Where(c => c.Day == dayToWatch && c.StartTime == startTime);

        Util.ClearResults();
        $"Last checked at {DateTime.Now.ToShortTimeString()}".Dump();
        fridayclasses.Dump();
        result = fridayclasses.Any(c => c.Availability != "0 Spaces");
        conn.Logout();
    }

    if (result)
    {
        //System.Media.SystemSounds.Exclamation.Play(); // Doesn't work if the user has turned system sounds off
        var sound = new System.Media.SoundPlayer(@"c:\windows\media\chimes.wav");
        sound.Play();
    }

    return result;
}