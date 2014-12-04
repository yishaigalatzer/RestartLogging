//// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
//// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

//using System;
//using System.Collections;
//using System.Globalization;
//using System.Text;
//using Microsoft.Framework.Logging;

//namespace RestartLogging.Logging
//{
//    public class FileLogger : ILogger
//    {
//        private const int _indentation = 2;
//        private readonly object _lock = new object();

//        public string FileName { get; }
//        public Func<string, LogLevel, bool> Filter { get; }

//        public FileLogger(string fileName, Func<string, LogLevel, bool> filter)
//        {
//            FileName = fileName;
//            Filter = filter ?? ((category, logLevel) => true);
//        }

//        public void Write(LogLevel logLevel, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
//        {
//            if (!IsEnabled(logLevel))
//            {
//                return;
//            }

//            var message = string.Empty;
//            var structure = state as ILoggerStructure;
//            if (structure != null)
//            {
//                var builder = new StringBuilder();
//                FormatLoggerStructure(
//                    builder,
//                    structure,
//                    level: 1,
//                    bullet: false);
//                message = Convert.ToString(builder.ToString(), CultureInfo.InvariantCulture);
//            }
//            else if (formatter != null)
//            {
//                message = formatter(state, exception);
//            }
//            else
//            {
//                if (state != null)
//                {
//                    message += state;
//                }
//                if (exception != null)
//                {
//                    message += Environment.NewLine + exception;
//                }
//            }
//            if (string.IsNullOrEmpty(message))
//            {
//                return;
//            }
//            lock (_lock)
//            {
//                var severity = logLevel.ToString().ToUpperInvariant();
//                File.WriteLine("[{0}{0}:{1}] {2}", severity, , message);
//            }
//        }

//        public bool IsEnabled(LogLevel logLevel)
//        {
//            return Filter(_name, logLevel);
//        }

//        public IDisposable BeginScope(object state)
//        {
//            return null;
//        }

//        private void FormatLoggerStructure(StringBuilder builder, ILoggerStructure structure, int level, bool bullet)
//        {
//            if (structure.Message != null)
//            {
//                builder.Append(structure.Message);
//            }
//            var values = structure.GetValues();
//            if (values == null)
//            {
//                return;
//            }
//            var isFirst = true;
//            foreach (var kvp in values)
//            {
//                builder.AppendLine();
//                if (bullet && isFirst)
//                {
//                    builder.Append(' ', level * _indentation - 1)
//                           .Append('-');
//                }
//                else
//                {
//                    builder.Append(' ', level * _indentation);
//                }
//                builder.Append(kvp.Key)
//                       .Append(": ");
//                if (kvp.Value is IEnumerable && !(kvp.Value is string))
//                {
//                    foreach (var value in (IEnumerable)kvp.Value)
//                    {
//                        if (value is ILoggerStructure)
//                        {
//                            FormatLoggerStructure(
//                                builder,
//                                (ILoggerStructure)value,
//                                level + 1,
//                                bullet: true);
//                        }
//                        else
//                        {
//                            builder.AppendLine()
//                                   .Append(' ', (level + 1) * _indentation)
//                                   .Append(value);
//                        }
//                    }
//                }
//                else if (kvp.Value is ILoggerStructure)
//                {
//                    FormatLoggerStructure(
//                        builder,
//                        (ILoggerStructure)kvp.Value,
//                        level + 1,
//                        bullet: false);
//                }
//                else
//                {
//                    builder.Append(kvp.Value);
//                }
//                isFirst = false;
//            }
//        }
//    }
//}