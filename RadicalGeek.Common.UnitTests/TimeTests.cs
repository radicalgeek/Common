using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RadicalGeek.Common.Time;

namespace RadicalGeek.Common.UnitTests
{
    [TestClass]
    public class TimeTests
    {
        [TestMethod]
        public void ClockNewDateConverts87Into1987()
        {
            DateTime result = Clock.NewDate(87, 1, 1);
            Assert.AreEqual(1987,result.Year);
        }

        [TestMethod]
        public void ClockNewDateConverts01Into2001()
        {
            Clock.SetTime(new DateTime(2020,1,1));
            DateTime result = Clock.NewDate(1, 1, 1);
            Assert.AreEqual(2001, result.Year);
        }
        
        [TestMethod]
        public void ClockNewDateConverts19Into2019WhenCurrentDateIsNotSpecified()
        {
            Clock.SetTime(new DateTime(2020,1,1));
            DateTime result = Clock.NewDate(19, 1, 1);
            Assert.AreEqual(2019, result.Year);
        }
        
        [TestMethod]
        public void ClockNewDateConverts19Into2019WhenCurrentDateIsSpecified()
        {
            DateTime result = Clock.NewDate(19, 1, 1, new DateTime(2020,1,1));
            Assert.AreEqual(2019, result.Year);
        }

        [TestMethod]
        public void ClockNewDateConverts22Into1922()
        {
            DateTime result = Clock.NewDate(22, 1, 1);
            Assert.AreEqual(1922, result.Year);
        }
    }
}
