using System;

namespace Shunty.GymMembershipManager
{
    public class BookedItem
    {
        public int Id { get; protected set; } = 0;
        public string Activity { get; protected set; } = "";
        public string Centre { get; protected set; } = "";
        public string Instructor { get; protected set; } = "";
        public string Location { get; protected set; } = "";
        public string Status { get; protected set; } = "";
        public DateTime StartDate { get; protected set; }
        public DateTime EndDate { get; protected set; }
        public TimeSpan StartTime { get; protected set; }
        public TimeSpan EndTime { get; protected set; }

        public bool CanCancel
        {
            get
            {
                return ((Id > 0) && (StartDate > DateTime.Now));
            }
        }

        public BookedItem()
        { }

        public BookedItem(string activityAndCentre)
            : this()
        {
            ParseActivityAndCentre(activityAndCentre);
        }

        public void ParseActivityAndCentre(string source)
        {
            var match = PatternMatchers.ActivityAndCentreRE.Match(source);
            if (match.Success)
            {
                Activity = match.Groups["activity"].Value.Trim();
                Centre = match.Groups["centre"].Value.Trim();
            }
        }

        public void ParseBookingDetails(string source)
        {
            var match = PatternMatchers.BookingDetailRE.Match(source);
            if (match.Success)
            {
                Instructor = match.Groups["instructor"].Value.Trim();
                Status = match.Groups["status"].Value.Trim();
                Location = match.Groups["location"].Value.Trim();

                ParseDateTime(match.Groups["datetime"].Value);
            }
        }

        private void ParseDateTime(string source)
        {
            var match = PatternMatchers.BookingDateTimeRE.Match(source);
            if (match.Success)
            {
                int fromHour = int.Parse(match.Groups["fromHour"].Value);
                int fromMin = int.Parse(match.Groups["fromMinute"].Value);
                int toHour = int.Parse(match.Groups["toHour"].Value);
                int toMin = int.Parse(match.Groups["toMinute"].Value);

                StartDate = DateTime.Parse(match.Groups["fullDate"].Value)
                    .AddHours(fromHour)
                    .AddMinutes(fromMin);
                EndDate = DateTime.Parse(match.Groups["fullDate"].Value)
                    .AddHours(toHour)
                    .AddMinutes(toMin);
                StartTime = new TimeSpan(fromHour, fromMin, 0);
                EndTime = new TimeSpan(toHour, toMin, 0);
                string idtxt = match.Groups["bookingId"].Success ? match.Groups["bookingId"].Value : "0";
                Id = int.Parse(idtxt);
            }
        }
    }
}
