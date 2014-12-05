using System.Collections.Generic;

namespace Combiner
{
    class EntryComparer : IComparer<Entry>
    {
        public int Compare(Entry x, Entry y)
        {
            return x.TimeStamp.CompareTo(y.TimeStamp);
        }
    }
}
