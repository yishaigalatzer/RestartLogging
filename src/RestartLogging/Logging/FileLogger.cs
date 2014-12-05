// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.IO;
using System.Text;
using Microsoft.Framework.Logging;

namespace RestartLogging.Logging
{
    public class FileLogger : ILogger
    {
        private int _indentation = 2;

        /// <summary>
        /// Logger name
        /// </summary>
        public string Name { get; }
        public string FileName { get; }
        public StreamWriter File { get; }

        public Func<string, LogLevel, bool> Filter { get; }

        public FileLogger(StreamWriter stream, string name, Func<string, LogLevel, bool> filter)
        {
            File = stream;
            Name = name;
            Filter = filter ?? ((category, logLevel) => true);
        }

        public void Write(object state)
        {
            Write(LogLevel.Verbose, 0, state, null, null);
        }

        public void Write(LogLevel logLevel, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var message = string.Empty;
            if (formatter != null)
            {
                message = formatter(state, exception);
            }
            else
            {
                if (state != null)
                {
                    message += state;
                }
                if (exception != null)
                {
                    message += Environment.NewLine + exception;
                }
            }
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            var severity = logLevel.ToString().ToUpperInvariant();
            File.WriteLine("{0}, {1}, {2}", DateTime.Now.ToString("hh:mm:ss:ffff"), "AppLog", message);
            File.Flush();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return Filter(Name, logLevel);
        }

        public IDisposable BeginScope(object state)
        {
            return new ScopeWriter(this, state);
        }

        private class ScopeWriter : IDisposable
        {
            private readonly FileLogger _logger;
            private readonly object _state;

            public ScopeWriter(FileLogger logger, object state)
            {
                _logger = logger;
                _logger.Write(state);
                _state = state;
            }
            public void Dispose()
            {
                _logger.Write(_state);
            }
        }

        private void FormatLoggerStructure(StringBuilder builder, ILoggerStructure structure, int level, bool bullet)
        {
            if (structure.Message != null)
            {
                builder.Append(structure.Message);
            }
            var values = structure.GetValues();
            if (values == null)
            {
                return;
            }
            var isFirst = true;
            foreach (var kvp in values)
            {
                builder.AppendLine();
                if (bullet && isFirst)
                {
                    builder.Append(' ', level * _indentation - 1)
                           .Append('-');
                }
                else
                {
                    builder.Append(' ', level * _indentation);
                }
                builder.Append(kvp.Key)
                       .Append(": ");
                if (kvp.Value is IEnumerable && !(kvp.Value is string))
                {
                    foreach (var value in (IEnumerable)kvp.Value)
                    {
                        if (value is ILoggerStructure)
                        {
                            FormatLoggerStructure(
                                builder,
                                (ILoggerStructure)value,
                                level + 1,
                                bullet: true);
                        }
                        else
                        {
                            builder.AppendLine()
                                   .Append(' ', (level + 1) * _indentation)
                                   .Append(value);
                        }
                    }
                }
                else if (kvp.Value is ILoggerStructure)
                {
                    FormatLoggerStructure(
                        builder,
                        (ILoggerStructure)kvp.Value,
                        level + 1,
                        bullet: false);
                }
                else
                {
                    builder.Append(kvp.Value);
                }
                isFirst = false;
            }
        }
    }
}