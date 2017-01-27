using System;
using System.Collections.Generic;

namespace Shunty.GymMembershipManager
{
    public class Timetable
    {
        private List<TimetableItem> _items = new List<TimetableItem>();

        public string Title { get; set; }
        public IEnumerable<TimetableItem> Items { get { return _items; } }

        public void AddItem(TimetableItem item)
        {
            _items.Add(item);
        }
    }
}
