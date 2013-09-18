using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using RadicalGeek.Common.Patterns.LazyLoading;
using RadicalGeek.Common.Structures;

namespace RadicalGeek.Common.UnitTests
{
    [TestClass()]
    public class LazyLoaderTests
    {
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LazyLoaderNullInitialiserTest()
        {
            LazyLoader<None> target = new LazyLoader<None>(null);
        }

        [TestMethod()]
        public void LazyLoaderValueTypeTest()
        {
            LazyLoader<int> target = new LazyLoader<int>(() => 1);
            Assert.AreEqual(1, (int)target);
        }

        [TestMethod()]
        public void LazyLoaderNonValueTypeTest()
        {
            LazyLoader<string> target = new LazyLoader<string>(() => new string(new[] { 'a', 'b', 'c' }));
            Assert.AreEqual("abc", (string)target);
        }
        
        [TestMethod()]
        public void LazyLoaderImplicitCastTest()
        {
            LazyLoader<string> target = new LazyLoader<string>(() => new string(new[] { 'a', 'b', 'c' }));
            Assert.AreEqual("abc", (string)target);
            target = "Hello";
            Assert.AreEqual("Hello", (string)target);
        }
    }
}
