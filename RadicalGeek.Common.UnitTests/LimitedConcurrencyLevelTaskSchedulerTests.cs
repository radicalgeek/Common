using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Threading;
using RadicalGeek.Common.Xml;
using RadicalGeek.Common.Concurrency;

namespace RadicalGeek.Common.UnitTests
{
    /// <summary>
    /// Summary description for LimitedConcurrencyLevelTaskSchedulerTests
    /// </summary>
    [TestClass]
    public class LimitedConcurrencyTests : IDisposable
    {

        private CancellationTokenSource _TokenSource;
        private CancellationToken _Token;
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                if (_TokenSource != null)
                {
                    _TokenSource.Dispose();
                    _TokenSource = null;
                }
        }
        ~LimitedConcurrencyTests()
        {
            Dispose(false);
        }

#region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize()
        {
            _TokenSource = new CancellationTokenSource();
            _Token = _TokenSource.Token;
        }
        //
        // Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            _TokenSource.Cancel();

        }
        //
        #endregion

        [TestMethod]
        public void LimitedConcurrencySchedulerOnlyAllows1Task()
        {
            int numberOfConcurrentTasks = 0;
            LimitedConcurrencyLevelTaskScheduler scheduler = new LimitedConcurrencyLevelTaskScheduler(1);
            List<Task> tasks = new List<Task>();
            TaskFactory factory = new TaskFactory(scheduler);
            for (int i = 0; i < 10; i++)
            {
                tasks.Add(factory.StartNew(() =>
        {

            numberOfConcurrentTasks++;
            Thread.Sleep(1000);

        }, _Token));
            }
            Thread.Sleep(50);
            Assert.AreEqual(1, numberOfConcurrentTasks);
        }

        [TestMethod,Ignore]
        public void LimitedConcurrencySchedulerOnlyAllows2Tasks()
        {
            int numberOfConcurrentTasks = 0;
            LimitedConcurrencyLevelTaskScheduler scheduler = new LimitedConcurrencyLevelTaskScheduler(2);
            List<Task> tasks = new List<Task>();
            TaskFactory factory = new TaskFactory(scheduler);
            for (int i = 0; i < 10; i++)
            {
                tasks.Add(factory.StartNew(() =>
                {
                    numberOfConcurrentTasks++;
                    Thread.Sleep(1000);
                }, _Token));
            }
            Thread.Sleep(50);
            Assert.AreEqual(2, numberOfConcurrentTasks);
        }

        [TestMethod,Ignore]
        public void LimitedConcurrencyTaskFactoryOnlyAllows5Tasks()
        {
            int numberOfConcurrentTasks = 0;
            List<Task> tasks = new List<Task>();
            TaskFactory factory = new LimitedConcurrencyTaskFactory(5);
            for (int i = 0; i < 20; i++)
            {
                tasks.Add(factory.StartNew(() =>
                {
                    numberOfConcurrentTasks++;
                    Thread.Sleep(1000);
                }, _Token));
            }
            Thread.Sleep(50);
            Assert.AreEqual(5, numberOfConcurrentTasks);
        }
    }
}
