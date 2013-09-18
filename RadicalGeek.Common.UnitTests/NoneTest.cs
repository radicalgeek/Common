using Microsoft.VisualStudio.TestTools.UnitTesting;
using RadicalGeek.Common.Structures;

namespace RadicalGeek.Common.UnitTests
{
    [TestClass]
    public class NoneTest
    {
        [TestMethod]
        public void CheckSizeOfNone()
        {
            unsafe
            {
                Assert.AreEqual(1, sizeof(None));
            }
        }

        [TestMethod]
        public void CheckValueOfNone()
        {
            Assert.AreEqual(default(None), None.Value);
            Assert.AreNotEqual(null,None.Value);
        }
    }
}
