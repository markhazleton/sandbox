using System;
using System.Linq;

namespace LINQSamples
{
    public class ItemCounter
    {
        public int UnreadCount { get; set; }
        public int TotalCount { get; set; }
    }

    public class CounterList
    {
        public CounterList()
        {
            Items = new List<ItemCounter>()
            {
                new ItemCounter() { TotalCount = 10, UnreadCount=5  },
                new ItemCounter() { TotalCount = 10, UnreadCount=5  },
                new ItemCounter() { TotalCount = 10, UnreadCount=5  },
                new ItemCounter() { TotalCount = 10, UnreadCount=5  }
            };
        }
        public List<ItemCounter> Items { get; set; }
    }


}
