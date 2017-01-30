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
        public static readonly string CancelUrl = "/hillingdon_-_highgro/BookingsCentre/CancelBooking";  // then add "?bookingid=12345678";
        public static readonly string CancelConfirmationUrl = "/hillingdon_-_highgro/BookingsCentre/CancelBookingConfirmation";  // then add "?bookingid=12345678";
        public static readonly string AddBookingUrl = "/hillingdon_-_highgro/BookingsCentre/AddBooking"; // add "?booking=13201988 (and optionally(?) "&ajax=0.9751552336996783");
        public static readonly string BasketUrl = "/hillingdon_-_highgro/Basket/Index";
        public static readonly string BasketPayUrl = "/hillingdon_-_highgro/Basket/Pay";
        public static readonly string BasketPaymentConfirmedUrl = "/hillingdon_-_highgro/basket/paymentconfirmed";
        public static readonly string BookingMessageUrl = "/hillingdon_-_highgro/BookingsCentre/Message"; // then add the json booking result as query params 
                                                                                                          // eg ?Success=true&AllowRetry=false
                                                                                                          //   &Message=Booking+added+to+basket
                                                                                                          //   &StartTime=Thu%2C+02+Feb+2017+06%3A45%3A00+GMT
                                                                                                          //   &EndTime=Thu%2C+02+Feb+2017+07%3A30%3A00+GMT
                                                                                                          //   &FacilityName=Highgrove
                                                                                                          //   &ActivityName=Group+Cycle+
                                                                                                          //   &ResourceLocation=Studio+2
                                                                                                          //   &KeepThis=true&"

        // Hard code the club id. We should really determine this from the page
        public static readonly int ClubId = 222;

        // Hard code this value. We should really look it up from the /hillingdon_-_highgro/bookingscentre/behaviours url
        // but we'll just assume it stays the same for the time being
        public static readonly int FitnessClassesBehaviour = 29;
    }
}
