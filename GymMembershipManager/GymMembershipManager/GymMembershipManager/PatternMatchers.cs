using System;
using System.Text.RegularExpressions;

namespace Shunty.GymMembershipManager
{
    public static class PatternMatchers
    {
        // Bookings/timetables
        private static readonly string BookingLinkPattern = @"onclick=['\""]addBooking\((?<id>\d+)\)";
        private static readonly string TimePattern = @"(?<fromHour>\d{1,2}):(?<fromMinute>\d{1,2})\s*-\s*(?<toHour>\d{1,2}):(?<toMinute>\d{1,2})";
        private static Regex _BookingLinkRE = null;
        private static Regex _TimeRE = null;

        // Booked items
        private static readonly string ActivityAndCentrePattern = @"(?<activity>[\w\s]*)\s+\((?<centre>[\w\s]*)\)";
        private static readonly string BookingDetailPattern = @"Instructor:\s*(?<instructor>[\w\s]*)\s+\((?<status>[\w\s]*)\)\<br[\s\/]*\>Location:\s+(?<location>[\w\d\s]*)\<br[\s\/]*\>(?<datetime>.*)";
        public static readonly string BookingDateTimePattern = @"Date: (?<fullDate>(?<day>\d{1,2})\s+(?<month>\w*)\s+(?<year>\d{4}))\s+" + TimePattern + @"([\s-]{0,3}\<a href=.*bookingid=(?<bookingId>\d*).*\</a\>)*";
        private static Regex _ActivityAndCentreRE = null;
        private static Regex _BookingDetailRE = null;
        private static Regex _BookingDateTimeRE = null;

        public static Regex BookingLinkRE
        {
            get
            {
                if (_BookingLinkRE == null)
                {
                    _BookingLinkRE = new Regex(BookingLinkPattern);
                }
                return _BookingLinkRE;
            }
        }

        public static Regex TimeRE
        {
            get
            {
                if (_TimeRE == null)
                {
                    _TimeRE = new Regex(TimePattern);
                }
                return _TimeRE;
            }
        }

        public static Regex ActivityAndCentreRE
        {
            get
            {
                if (_ActivityAndCentreRE == null)
                {
                    _ActivityAndCentreRE = new Regex(ActivityAndCentrePattern);
                }
                return _ActivityAndCentreRE;
            }
        }

        public static Regex BookingDetailRE
        {
            get
            {
                if (_BookingDetailRE == null)
                {
                    _BookingDetailRE = new Regex(BookingDetailPattern);
                }
                return _BookingDetailRE;
            }
        }

        public static Regex BookingDateTimeRE
        {
            get
            {
                if (_BookingDateTimeRE == null)
                {
                    _BookingDateTimeRE = new Regex(BookingDateTimePattern);
                }
                return _BookingDateTimeRE;
            }
        }

    }
}
