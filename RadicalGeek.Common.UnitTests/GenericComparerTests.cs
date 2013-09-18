using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RadicalGeek.Common.UnitTests
{
    [TestClass]
    public class GenericComparerTests
    {
        [TestMethod]
        public void GenericComparerAsIComparer()
        {
            List<int> ints = new List<int>(new[] { 10, 5, 2, 23, 7, 5, 3, 45, 23, 64, 25 });

            ints.Sort(new GenericComparer<int>());

            Assert.AreEqual(ints.Min(), ints.First());
            Assert.AreEqual(ints.Max(), ints.Last());

            ints.Sort(new GenericComparer<int>((i, i1) => Math.Sin(i) > Math.Sin(i1) ? -1 : Math.Sin(i) < Math.Sin(i1) ? 1 : 0));

            Assert.AreEqual(64, ints.First());
            Assert.AreEqual(5, ints.Last());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GenericComparerAsIEqualityComparer()
        {
            Dictionary<Box, String> boxes = new Dictionary<Box, string>(
                new GenericComparer<Box>(
                    b => b.Height ^ b.Length ^ b.Width,
                    (b1, b2) => b1.Height == b2.Height && b1.Width == b2.Width && b1.Length == b2.Length));

            Box redBox = new Box(4, 3, 4);
            Box blueBox = new Box(4, 3, 4);

            boxes.Add(redBox, "red");
            boxes.Add(blueBox, "blue");
        }

        private class Box
        {
            public Box(int h, int l, int w)
            {
                Height = h;
                Length = l;
                Width = w;
            }
            public int Height { get; private set; }
            public int Length { get; private set; }
            public int Width { get; private set; }
        }

    }
}
