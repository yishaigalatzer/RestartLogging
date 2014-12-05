using System;

namespace Combiner
{
    public class Entry
    {
        public TimeSpan TimeFromStart { get; set; }
        public DateTime TimeStamp { get; set; }
        public Origin Origin { get; set; }
        public string Message { get; set; }
    }
}
