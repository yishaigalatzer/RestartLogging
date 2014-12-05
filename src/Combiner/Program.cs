using System;
using System.IO;
using System.Linq;

namespace Combiner
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0 || args.Length > 3 || args.Length == 2 || (args.Length == 3 && args[1] != "-o"))
            {
                Usage();
                return;
            }

            string baseFileName = GetBaseFileName(args[0]);

            if (!AnyOfFiles(baseFileName))
            {
                Usage();
                return;
            }

            string outputFileName = baseFileName + ".combined.log";
            if (args.Length == 3)
            {
                outputFileName = args[2];
            }

            File.Delete(outputFileName);

            var entries = Combiner.Combine(baseFileName);

            Writer.Write(entries, outputFileName);
        }

        public static string GetBaseFileName(string fileName)
        {
            return fileName.TrimEnd(".log").TrimEnd(".vs").TrimEnd(".app").TrimEnd(".dth");
        }

        public static bool AnyOfFiles(string baseFileName)
        {
            var files = Enum.GetNames(typeof(Origin)).Select(n => baseFileName + "." + n + ".log");
            return files.Any(f => File.Exists(f));
        }

        public static void Usage()
        {
            Console.WriteLine("Usage: Combiner AppName");
            Console.WriteLine();
            Console.WriteLine("Usage: Combiner AppName [-o outputFile]");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Will combine AppName.app.log AppName.vs.log and AppName.dth.log into output");
            Console.WriteLine("default for output is AppName.combined.log");
        }
    }
}
