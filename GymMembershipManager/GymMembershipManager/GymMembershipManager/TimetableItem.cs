using System;

namespace Shunty.GymMembershipManager
{
    public class TimetableItem
    {
        public int Id { get; set; } = 0;
        public bool Enrolled { get; set; } = false;
        public string Centre { get; set; }
        public string Day { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Availability { get; set; }
        public string Instructor { get; set; }
    }
}
