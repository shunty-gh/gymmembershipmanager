using System;
using System.Collections.Generic;

namespace Shunty.GymMembershipManager
{
    public class BookedItems
    {
        private List<BookedItem> _items = new List<BookedItem>();

        public IEnumerable<BookedItem> Bookings { get { return _items; } }

        public BookedItem AddNew(string activityAndCentre)
        {
            var result = new BookedItem(activityAndCentre);
            _items.Add(result);
            return result;
        }
    }
}
