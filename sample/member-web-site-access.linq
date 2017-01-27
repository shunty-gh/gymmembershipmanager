<Query Kind="Program">
  <Reference>&lt;ProgramFilesX86&gt;\Microsoft ASP.NET\ASP.NET MVC 4\Assemblies\System.Net.Http.dll</Reference>
  <Reference>&lt;ProgramFilesX86&gt;\Microsoft ASP.NET\ASP.NET MVC 4\Assemblies\System.Net.Http.Formatting.dll</Reference>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Net</Namespace>
</Query>

static async void Main() 
{ 
    string baseaddress = "https://better.legendonlineservices.co.uk";
    string loginurl = "/hillingdon_-_highgro/account/login";
    string logouturl = "/hillingdon_-_highgro/Account/Logout";
    string bookingsurl = "/hillingdon_-_highgro/BookingsCentre/Index";
    string mybookingsurl = "/hillingdon_-_highgro/BookingsCentre/MyBookings";
    string ticketsurl = "/hillingdon_-_highgro/onlineticketing/browse";

    string user = Util.GetPassword("BetterGym-User");
    string pwd = Util.GetPassword("BetterGym");

    var cookies = new CookieContainer();
    var handler = new HttpClientHandler
    {
        AllowAutoRedirect = true,
        UseCookies = true,
        CookieContainer = cookies
    };
    
    using (var client = new HttpClient(handler))
    {
        client.BaseAddress = new Uri(baseaddress);

        // First make a GET call to the site to get initial cookies
        var resultInitial = await client.GetAsync(loginurl);
        Util.OnDemand("Initial call", () => resultInitial.Dump());
        Util.OnDemand("Initial cookies", () => cookies.Dump());
        resultInitial.EnsureSuccessStatusCode();

        // Then make the login request
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("login.Email", user),
            new KeyValuePair<string, string>("login.Password", pwd),
            new KeyValuePair<string, string>("login.RedirectURL", ""),
        });
        var result = await client.PostAsync(loginurl, content);
        Util.OnDemand("Login call", () => result.Dump());
        result.EnsureSuccessStatusCode();
        Util.OnDemand("Login cookies", () => cookies.Dump());
        var resultContent = await result.Content.ReadAsStringAsync();
        Util.OnDemand("Member login page", () => resultContent.Dump());

        // Get my current bookings
        var resultBookings = await client.GetAsync(mybookingsurl);
        //Util.OnDemand("My bookings call", () => resultBookings.Dump());
        resultBookings.Dump();
        resultBookings.EnsureSuccessStatusCode();
        var resultBookingsContent = await resultBookings.Content.ReadAsStringAsync();
        //Util.OnDemand("My bookings", () => resultBookingsContent.Dump());
        resultBookingsContent.Dump();

        // Logout
//        var resultLogout = await client.GetAsync(logouturl);
//        Util.OnDemand("Logout call", () => resultLogout.Dump());
//        var resultLogoutContent = await resultLogout.Content.ReadAsStringAsync();
//        Util.OnDemand("Logout", () => resultLogoutContent.Dump());

    }

}