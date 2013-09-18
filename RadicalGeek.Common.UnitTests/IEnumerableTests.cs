using System;
using System.Collections.Generic;
using System.Linq;
using RadicalGeek.Common.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RadicalGeek.Common.UnitTests
{
    [TestClass]
    public class IEnumerableTests
    {
        [TestMethod]
        public void FindIndexFindsFirstItemOfTwo()
        {
            List<int> testList = new List<int> { 1, 3 };
            int[] result = testList.FindIndexBinary(1);
            Assert.AreEqual(0, result[0]);
            Assert.AreEqual(0, result[1]);
        }

        [TestMethod]
        public void FindIndexFindsSecondItemOfTwo()
        {
            List<int> testList = new List<int> { 1, 3 };
            int[] result = testList.FindIndexBinary(3);
            Assert.AreEqual(1, result[0]);
            Assert.AreEqual(1, result[1]);
        }

        [TestMethod]
        public void FindIndexFindsFirstAndSecondItemsInTwo()
        {
            List<int> testList = new List<int> { 1, 3 };
            int[] result = testList.FindIndexBinary(2);
            Assert.AreEqual(0, result[0]);
            Assert.AreEqual(1, result[1]);
        }

        [TestMethod]
        public void FindIndexPastEndWith2Items()
        {
            List<int> testList = new List<int> { 1, 3 };
            int[] result = testList.FindIndexBinary(4);
            Assert.AreEqual(1, result[0]);
            Assert.AreEqual(2, result[1]);
        }

        [TestMethod]
        public void FindIndexFindsThirdItemOfFive()
        {
            List<int> testList = new List<int> { 1, 3, 5, 7, 9 };
            int[] result = testList.FindIndexBinary(5);
            Assert.AreEqual(2, result[0]);
            Assert.AreEqual(2, result[1]);
        }

        [TestMethod]
        public void FindIndexFindsThirdAndFourthItemsOfFive()
        {
            List<int> testList = new List<int> { 1, 3, 5, 7, 9 };
            int[] result = testList.FindIndexBinary(6);
            Assert.AreEqual(2, result[0]);
            Assert.AreEqual(3, result[1]);
        }

        [TestMethod]
        public void FindIndexFindsFourthAndFifthItemsOfFive()
        {
            List<int> testList = new List<int> { 1, 3, 5, 7, 9 };
            int[] result = testList.FindIndexBinary(8);
            Assert.AreEqual(3, result[0]);
            Assert.AreEqual(4, result[1]);
        }

        [TestMethod]
        public void FindIndexPastEndWith5Items()
        {
            List<int> testList = new List<int> { 1, 3, 5, 7, 9 };
            int[] result = testList.FindIndexBinary(10);
            Assert.AreEqual(4, result[0]);
            Assert.AreEqual(5, result[1]);
        }

        [TestMethod]
        public void FindIndexPastStartWith5Items()
        {
            List<int> testList = new List<int> { 1, 3, 5, 7, 9 };
            int[] result = testList.FindIndexBinary(0);
            Assert.AreEqual(-1, result[0]);
            Assert.AreEqual(0, result[1]);
        }

        [TestMethod]
        public void FindIndexFindsFourthAndFifthItemsOfFiveOffsetArray()
        {
            Array testArray = Array.CreateInstance(typeof(int), new[] { 5 }, new[] { 3 });
            int[] inArray = new[] { 1, 3, 5, 7, 9 };
            Array.Copy(inArray, 0, testArray, 3, 5);
            int[] result = testArray.FindIndexBinary(8);
            Assert.AreEqual(6, result[0]);
            Assert.AreEqual(7, result[1]);
        }

        [TestMethod]
        public void FindIndexFindsFourthAndFifthItemsOfFiveNormalArray()
        {
            int[] inArray = new[] { 1, 3, 5, 7, 9 };
            int[] result = inArray.FindIndexBinary(8);
            Assert.AreEqual(3, result[0]);
            Assert.AreEqual(4, result[1]);
        }

        [TestMethod]
        public void FindIndexInLargeList()
        {
            Random random = new Random();
            List<int> randomList = new List<int>();
            for (int i = 0; i < 15000; i++)
            {
                randomList.Add(random.Next(1, 20000));
            }
            randomList.Sort();
            List<int> list = randomList.Distinct().ToList();

            for (int j = 0; j < 20000; j++)
            {
                int candidate = j;
                int[] findResult = list.FindIndexBinary(candidate);
                bool contains = list.Contains(candidate);
                Assert.AreEqual(contains, findResult[0] == findResult[1],
                                "List.Contains returned {0} but indexes were {1} and {2}", contains, findResult[0],
                                findResult[1]);
                if (contains)
                {
                    int index = list.IndexOf(candidate);
                    Assert.AreEqual(index, findResult[0], "List.IndexOf returned {0} but index 0 was {1}", index,
                                    findResult[0]);
                    Assert.AreEqual(index, findResult[1], "List.IndexOf returned {0} but index 1 was {1}", index,
                                    findResult[1]);
                }
                else
                {
                    {
                        while (!list.Contains(candidate) && candidate > 0)
                            candidate--;
                        int index = list.IndexOf(candidate);
                        Assert.AreEqual(index, findResult[0], "Preceding value's index was {0} but index 0 was {1}",
                                        index, findResult[0]);
                    }
                    candidate++;
                    {
                        while (!list.Contains(candidate) && candidate < 20000)
                            candidate++;
                        if (candidate != 20000)
                        {
                            int index = list.IndexOf(candidate);
                            Assert.AreEqual(index, findResult[1],
                                            "Following value's index was {0} but index 1 was {1}",
                                            index, findResult[1]);
                        }
                    }
                }
            }

        }
    }
}
