using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Combiner
{
    class Combiner
    {
        public const string DateTimeFormat = "hh:mm:ss:ffff";

        public static List<Entry> Combine(string basePath)
        {
            List<Entry> entries = new List<Entry>();

            foreach (var origin in Enum.GetValues(typeof(Origin)).Cast<Origin>())
            {
                string fileName = basePath + "." + origin.ToString() + ".log";

                if (File.Exists(fileName))
                {
                    var newEntries = Reader.ReadFile(fileName, origin);
                    entries.AddRange(newEntries);
                }

                entries.Sort(new EntryComparer());
            }

            var start = entries.FirstOrDefault()?.TimeStamp;

            if (start != null)
            {
                foreach (var entry in entries)
                {
                    entry.TimeFromStart = entry.TimeStamp - start.Value;
                }
            }

            return entries;
        }
    }
}
