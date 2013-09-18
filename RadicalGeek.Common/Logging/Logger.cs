using System;
using System.Configuration;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace RadicalGeek.Common.Logging
{
    public static class Logger
    {
        private static readonly object _LockObject = new object();

        private static bool _IsLoggingEnabled
        {
            get
            {
                string isLoggingEnabledValue = ConfigurationManager.AppSettings["IsLoggingEnabled"];

                bool isLoggingEnabled;
                bool isParsed = bool.TryParse(isLoggingEnabledValue, out isLoggingEnabled);

                return isParsed && isLoggingEnabled;
            }
        }

        private static string _EventLog
        {
            get
            {
                return ConfigurationManager.AppSettings["EventLog"];
            }
        }

        private static string _EventLogSource
        {
            get
            {
                return ConfigurationManager.AppSettings["EventLogSource"];
            }
        }

        /// <summary>
        /// Logs a message to a severity level.
        /// </summary>
        /// <param name="logEntry">Log entry to be logged</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "If the logEntry is null then this will be logged. We do not want logging errors to stop the user experience.")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Require derived LogEntry because this hides code that could have been duplicated otherwise.")]
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch any exceptions when logging and log to event log.")]
        public static void Log(LogEntry logEntry)
        {
            if (logEntry == null)
                Log("Log entry cannot be null.", LoggingSeverity.Error);

            if (_IsLoggingEnabled)
            {
                try
                {
                    lock (_LockObject)
                        Microsoft.Practices.EnterpriseLibrary.Logging.Logger.Write(logEntry);
                }
                catch (Exception exception)
                {
                    string msg = String.Format(CultureInfo.CurrentCulture, "Logging failed for: {0}\r\nSeverity: {1}\r\nMessage: {2}", logEntry.CategoriesStrings, logEntry.Severity, logEntry.Message);
                    WriteExceptionToEventLog(msg, exception);
                }
            }
        }

        private static void WriteExceptionToEventLog(string message, Exception exception)
        {
            using (EventLog eventLog = new EventLog())
            {
                eventLog.Log = _EventLog;
                eventLog.Source = _EventLogSource;
                eventLog.EnableRaisingEvents = true;
                string newMessage = String.Format(CultureInfo.CurrentCulture, "\r\n\r\n{0}\r\n\r\n{1}", message, exception);
                eventLog.WriteEntry(newMessage, EventLogEntryType.Error);
            }
        }

        /// <summary>
        /// Logs a LogEntry and exception to a default LoggingSeverity.Error.
        /// </summary>
        /// <param name="logEntry">Log entry to be logged</param>
        /// <param name="loggedException">Exception to be logged</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "If the logEntry is null then this will be logged. We do not want logging errors to stop the user experience.")]
        public static void Log(LogEntry logEntry, Exception loggedException)
        {
            if (logEntry == null)
                Log("Log entry cannot be null.", LoggingSeverity.Error);

            if (loggedException == null)
                Log("Log exception cannot be null.", LoggingSeverity.Error);

            string exceptionMessage = String.Format(CultureInfo.CurrentCulture, "{0}\r\nException: {1}", logEntry.Message, loggedException);
            logEntry.Message = exceptionMessage;
            logEntry.SetLoggingSeverity(LoggingSeverity.Error);

            Log(logEntry);
        }

        /// <summary>
        /// Logs a message and defaults to LoggingSeverity.Information.
        /// </summary>
        /// <param name="message">Message to be logged</param>
        public static void Log(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                Log("Log message cannot be null, empty or have whitespace.", LoggingSeverity.Error);

            LogEntry logEntry = new LogEntry();
            logEntry.Message = message;
            logEntry.SetLoggingSeverity(LoggingSeverity.Information);

            Log(logEntry);
        }

        /// <summary>
        /// Logs a message and exception to a severity level.
        /// </summary>
        /// <param name="message">Message to be logged</param>
        /// <param name="loggingSeverity">Severity level of message</param>
        public static void Log(string message, LoggingSeverity loggingSeverity)
        {
            if (string.IsNullOrWhiteSpace(message))
                Log("Log message cannot be null, empty or have whitespace.", LoggingSeverity.Error);

            LogEntry logEntry = new LogEntry();
            logEntry.Message = message;
            logEntry.SetLoggingSeverity(loggingSeverity);

            Log(logEntry);
        }

        /// <summary>
        /// Logs a message and exception to the default LoggingSeverity.Error.
        /// </summary>
        /// <param name="message">Message to be logged</param>
        /// <param name="loggedException">Exception to be logged</param>
        public static void Log(string message, Exception loggedException)
        {
            if (string.IsNullOrWhiteSpace(message))
                Log("Log message cannot be null, empty or have whitespace.", LoggingSeverity.Error);

            if (loggedException == null)
                Log("Log exception cannot be null.", LoggingSeverity.Error);

            LogEntry logEntry = new LogEntry();
            string exceptionMessage = String.Format(CultureInfo.CurrentCulture, "{0}\r\nException: {1}", message, loggedException);
            logEntry.Message = exceptionMessage;
            logEntry.SetLoggingSeverity(LoggingSeverity.Error);

            Log(logEntry);
        }

        /// <summary>
        /// Logs a message and exception to a severity level.
        /// </summary>
        /// <param name="message">Message to be logged</param>
        /// <param name="loggingSeverity">Severity level of message</param>
        /// <param name="loggedException">Exception to be logged</param>
        public static void Log(string message, LoggingSeverity loggingSeverity, Exception loggedException)
        {
            if (string.IsNullOrWhiteSpace(message))
                Log("Log message cannot be null, empty or have whitespace.", LoggingSeverity.Error);

            if (loggedException == null)
                Log("Log exception cannot be null.", LoggingSeverity.Error);

            LogEntry logEntry = new LogEntry();
            string exceptionMessage = String.Format(CultureInfo.CurrentCulture, "{0}\r\nException: {1}", message, loggedException);
            logEntry.Message = exceptionMessage;
            logEntry.SetLoggingSeverity(loggingSeverity);

            Log(logEntry);
        }
    }
}
