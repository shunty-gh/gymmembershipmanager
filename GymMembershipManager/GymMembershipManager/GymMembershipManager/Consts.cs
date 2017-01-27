using System;

namespace Shunty.GymMembershipManager
{
    public static class Consts
    {
        public static string BaseAddress = "https://better.legendonlineservices.co.uk";
        public static string LoginUrl = "/hillingdon_-_highgro/account/login";
        public static string LogoutUrl = "/hillingdon_-_highgro/Account/Logout";
        public static string MakeBookingUrl = "/hillingdon_-_highgro/BookingsCentre/Index";
        public static string MyBookingsUrl = "/hillingdon_-_highgro/BookingsCentre/MyBookings";
        public static string TicketsUrl = "/hillingdon_-_highgro/onlineticketing/browse";
        public static string ActivitiesUrl = "/hillingdon_-_highgro/bookingscentre/activities";
        public static string ActivitySelectUrl = "/hillingdon_-_highgro/bookingscentre/activitySelect";
        public static string BehavioursUrl = "/hillingdon_-_highgro/bookingscentre/behaviours";
        public static string TimetableUrl = "/hillingdon_-_highgro/bookingscentre/TimeTable";

        // Hard code the club id. We should really determine this from the page
        public static int ClubId = 222;

        // Hard code this value. We should really look it up from the /hillingdon_-_highgro/bookingscentre/behaviours url
        // but we'll just assume it stays the same for the time being
        public static int FitnessClassesBehaviour = 29;
    }
}
