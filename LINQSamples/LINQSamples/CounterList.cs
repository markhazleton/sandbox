namespace LINQSamples
{
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


