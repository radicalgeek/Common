using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RadicalGeek.Common.Collections;

namespace RadicalGeek.Common.UnitTests
{
    [TestClass]
    public class OrderedActionQueueTests
    {
        [TestMethod]
        public void TestQueuedMethodsSuccessfulAfterRetries()
        {
            List<string> log = new List<string>();
            OrderedActionQueue queue = new OrderedActionQueue { WriteLog = log.Add };
            int m = -2;
            queue.Add(new List<Action>
                {
                ()=>log.Add(string.Format("{0} {1}", "Hello", "World")),
                () =>
                    {
                        m++;
                        log.Add(string.Format("Do 1 / {0}", Math.Max(0, m)));
                        log.Add((1 / Math.Max(0,m)).ToString());
                        log.Add("Done");
                    }
            });

            while (!log.Contains("Done"))
            {
                Thread.Sleep(100);
            }

            Assert.AreEqual("Hello World", log[0]);
            Assert.AreEqual("Do 1 / 0", log[1]);
            Assert.AreEqual("Do 1 / 0", log[2]);
            Assert.AreEqual("Do 1 / 1", log[3]);
            Assert.AreEqual("1", log[4]);
            Assert.AreEqual("Done", log[5]);
        }
    }
}
