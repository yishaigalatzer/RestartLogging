using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Combiner
{
    public class Reader
    {
        public static IEnumerable<Entry> ReadFile(string filePath, Origin origin)
        {
            var entries = new List<Entry>();

            List<string> lines = new List<string>();
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                string line = string.Empty;
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    if (line != null)
                    {
                        lines.Add(line);
                    }
                }
            }

            for (int lineNumber = 0; lineNumber < lines.Count; lineNumber++)
            {
                string line = lines[lineNumber];
                var split = line.Split(new[] { ',' });

                if (split.Length >= 3)
                {
                    DateTime dt;
                    if (!DateTime.TryParseExact(split[0], Combiner.DateTimeFormat, null, DateTimeStyles.None, out dt))
                    {
                        throw new InvalidOperationException(string.Format("Invalid date in File: {0}, Line: {1} Date: {2} Text: {3} ",
                            filePath,
                            lineNumber + 1,
                            split[0],
                            line));
                    }

                    string message = string.Join(", ", split.Skip(2));

                    var entry = new Entry()
                    {
                        Origin = origin,
                        TimeStamp = dt,
                        Message = split[2],
                    };

                    entries.Add(entry);
                }
            }

            return entries;
        }
    }
}
