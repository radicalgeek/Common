using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;

namespace RadicalGeek.Common.Logging
{
    public class LogEntry : Microsoft.Practices.EnterpriseLibrary.Logging.LogEntry
    {
        public LogEntry() { }

        public LogEntry(string message, Collection<string> categories, LoggingSeverity loggingSeverity)
        {
            Message = message;
            Severity = GetLoggingSeverity(loggingSeverity);
            TimeStamp = DateTime.Now;
            Categories = new List<string>(categories);
        }

        public LogEntry(string message, Exception exception, LoggingSeverity loggingSeverity)
        {
            if (exception == null)
                throw new ArgumentNullException("exception", "Exception object to log cannot be null.");

            Message = String.Format(CultureInfo.CurrentCulture, "{0}\r\nException: {1}", message, exception);
            Severity = GetLoggingSeverity(loggingSeverity);
            TimeStamp = DateTime.Now;
            Categories = new List<string> { exception.Source };
        }

        public void SetLoggingSeverity(LoggingSeverity loggingSeverity)
        {
            Severity = GetLoggingSeverity(loggingSeverity);
        }

        private static TraceEventType GetLoggingSeverity(LoggingSeverity loggingSeverity)
        {
            TraceEventType severity;

            switch (loggingSeverity)
            {
                case LoggingSeverity.Critical:
                    severity = TraceEventType.Critical;
                    break;
                case LoggingSeverity.Error:
                    severity = TraceEventType.Error;
                    break;
                case LoggingSeverity.Warning:
                    severity = TraceEventType.Warning;
                    break;
                case LoggingSeverity.Information:
                    severity = TraceEventType.Information;
                    break;
                case LoggingSeverity.Debug:
                    severity = TraceEventType.Verbose;
                    break;
                default:
                    severity = TraceEventType.Information;
                    break;
            }

            return severity;
        }

    }
}
