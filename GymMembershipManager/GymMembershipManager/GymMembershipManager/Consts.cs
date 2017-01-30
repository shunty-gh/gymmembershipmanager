using System;

namespace Shunty.GymMembershipManager
{
    public static class Consts
    {
        public static readonly string BaseAddress = "https://better.legendonlineservices.co.uk";
        public static readonly string LoginUrl = "/hillingdon_-_highgro/account/login";
        public static readonly string LogoutUrl = "/hillingdon_-_highgro/Account/Logout";
        public static readonly string MakeBookingUrl = "/hillingdon_-_highgro/BookingsCentre/Index";
        public static readonly string MyBookingsUrl = "/hillingdon_-_highgro/BookingsCentre/MyBookings";
        public static readonly string TicketsUrl = "/hillingdon_-_highgro/onlineticketing/browse";
        public static readonly string ActivitiesUrl = "/hillingdon_-_highgro/bookingscentre/activities";
        public static readonly string ActivitySelectUrl = "/hillingdon_-_highgro/bookingscentre/activitySelect";
        public static readonly string BehavioursUrl = "/hillingdon_-_highgro/bookingscentre/behaviours";
        public static readonly string TimetableUrl = "/hillingdon_-_highgro/bookingscentre/TimeTable";

        // Hard code the club id. We should really determine this from the page
        public static readonly int ClubId = 222;

        // Hard code this value. We should really look it up from the /hillingdon_-_highgro/bookingscentre/behaviours url
        // but we'll just assume it stays the same for the time being
        public static readonly int FitnessClassesBehaviour = 29;
    }
}
