using System;
using System.IO;
using Microsoft.Framework.Logging;

namespace RestartLogging.Logging
{
    public class FileLoggerProvider : ILoggerProvider
    {
        private readonly StreamWriter Writer;
        private readonly Func<string, LogLevel, bool> _filter;

        public FileLoggerProvider(StreamWriter writer, Func<string, LogLevel, bool> filter)
        {
            _filter = filter;
            Writer = writer;
        }

        public ILogger Create(string name)
        {
            return new FileLogger(Writer, name, _filter);
        }
    }
}