using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;
using RadicalGeek.Common.Logging;

namespace RadicalGeek.Common.UnitTests.Logging
{
    [TestClass]
    public class LogEntryTests
    {
        #region Constructor

        [TestMethod]
        public void ConstructorPopulatesMessageProperty()
        {
            LogEntry entry = new LogEntry("fake message", new Collection<string>(), LoggingSeverity.Information);

            Assert.AreEqual("fake message", entry.Message);
        }

        [TestMethod]
        public void ConstructorPopulatesTimeStampProperty()
        {
            var entry = new LogEntry("fake message", new Collection<string>(), LoggingSeverity.Information);

            Assert.AreEqual(DateTime.Today.ToString("dd/MM/yyyy", CultureInfo.CurrentCulture), entry.TimeStamp.ToString("dd/MM/yyyy", CultureInfo.CurrentCulture));
        }

        [TestMethod]
        public void ConstructorPopulatesCategoriesProperty()
        {
            var entry = new LogEntry("fake message", new Collection<string> { "CreditScoringService" }, LoggingSeverity.Information);

            Assert.IsTrue(entry.Categories.Contains("CreditScoringService"));
        }

        [TestMethod]
        public void ConstructorPopulatesSeverityPropertyAsInformation()
        {
            var entry = new LogEntry("fake message", new Collection<string>(), LoggingSeverity.Information);

            Assert.AreEqual(TraceEventType.Information, entry.Severity);
        }

        [TestMethod]
        public void ConstructorPopulatesSeverityPropertyAsError()
        {
            var entry = new LogEntry("fake message", new Collection<string>(), LoggingSeverity.Error);

            Assert.AreEqual(TraceEventType.Error, entry.Severity);
        }

        [TestMethod]
        public void ConstructorPopulatesSeverityPropertyAsVerbose()
        {
            var entry = new LogEntry("fake message", new Collection<string>(), LoggingSeverity.Debug);

            Assert.AreEqual(TraceEventType.Verbose, entry.Severity);
        }

        [TestMethod]
        public void ConstructorPopulatesSeverityPropertyAsWarning()
        {
            var entry = new LogEntry("fake message", new Collection<string>(), LoggingSeverity.Warning);

            Assert.AreEqual(TraceEventType.Warning, entry.Severity);
        }

        [TestMethod]
        public void ConstructorPopulatesSeverityPropertyAsCritical()
        {
            var entry = new LogEntry("fake message", new Collection<string>(), LoggingSeverity.Critical);

            Assert.AreEqual(TraceEventType.Critical, entry.Severity);
        }

        [TestMethod]
        public void ConstructorPopulatesSeverityPropertyAsDefault()
        {
            var entry = new LogEntry("fake message", new Collection<string>(), (LoggingSeverity)999);

            Assert.AreEqual(TraceEventType.Information, entry.Severity);
        }

        #endregion

        #region Constructor with Exception

        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "DollarFinancialServicesFramework.Logging.LogEntry", Justification = "Not using the LogEntry object because not necessary for this test.")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorWithExceptionThrowsArgumentNullExceptionIfExceptionIsNull()
        {
            try
            {
                new LogEntry("fake message", (Exception)null, LoggingSeverity.Error);
            }
            catch (ArgumentNullException exception)
            {
                Assert.AreEqual("Exception object to log cannot be null.\r\nParameter name: exception", exception.Message);
                throw;
            }
        }

        [TestMethod]
        public void ConstructorWithExceptionPopulatesMessageProperty()
        {
            var entry = new LogEntry("fake message", new InvalidOperationException(), LoggingSeverity.Error);

            Assert.AreEqual("fake message\r\nException: System.InvalidOperationException: Operation is not valid due to the current state of the object.", entry.Message);
        }

        [TestMethod]
        public void ConstructorWithExceptionPopulatesTimeStampProperty()
        {
            var entry = new LogEntry("fake message", new InvalidOperationException(), LoggingSeverity.Error);

            Assert.AreEqual(DateTime.Today.ToString("dd/MM/yyyy", CultureInfo.CurrentCulture), entry.TimeStamp.ToString("dd/MM/yyyy", CultureInfo.CurrentCulture));
        }

        [TestMethod]
        public void ConstructorWithExceptionPopulatesCategoriesProperty()
        {
            var exception = new InvalidOperationException() { Source = "FakeService" };

            var entry = new LogEntry("fake message", exception, LoggingSeverity.Error);

            Assert.IsTrue(entry.Categories.Contains(exception.Source));
        }

        #endregion

        [TestMethod]
        public void SetLoggingSeveritySetsTheSeverity()
        {
            var entry = new LogEntry();
            entry.SetLoggingSeverity(LoggingSeverity.Information);

            Assert.AreEqual(entry.Severity, TraceEventType.Information);
        }
    }
}
