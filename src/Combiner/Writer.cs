using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Combiner
{
    class Writer
    {
        public static void Write(List<Entry> entries, string outputFileName)
        {
            using (var ostream = File.OpenWrite(outputFileName))
            using (var writer = new StreamWriter(ostream, Encoding.UTF8))
            {
                foreach (var entry in entries)
                {
                    string line = string.Format("{0}, {1}, {2}", entry.TimeFromStart.ToString(@"mm\.ss\.fff"),
                                                                 entry.Origin.ToString(),
                                                                 entry.Message);
                    writer.WriteLine(line);
                }
            }
        }
    }
}
