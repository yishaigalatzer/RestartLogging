using System;
using System.IO;
using System.Text;
using RestartLogging.Logging;

namespace Microsoft.Framework.Logging
{
    public static class FileLoggerExtensions
    {
        /// <summary>
        /// Adds a console logger that is enabled for <see cref="LogLevel"/>.Information or higher.
        /// </summary>
        public static ILoggerFactory AddFile(this ILoggerFactory factory, string filePath, Func<string, LogLevel, bool> filter = null)
        {
            filter = filter ?? DefaultFilter;

            var stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite, 1024, false);
            var writer = new StreamWriter(stream, Encoding.ASCII);

            factory.AddProvider(
                new FileLoggerProvider(writer, (category, logLevel) => category == "timing"));

            return factory;
        }

        private static bool DefaultFilter(string category, LogLevel level)
        {
            return string.Equals(category, "timing", StringComparison.OrdinalIgnoreCase);
        }
    }
}