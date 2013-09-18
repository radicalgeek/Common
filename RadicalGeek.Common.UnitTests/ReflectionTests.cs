using System;
using RadicalGeek.Common.Reflection;
using RadicalGeek.Common.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Timers;

namespace RadicalGeek.Common.UnitTests
{
    [TestClass]
    public class ReflectionTests
    {
        private interface ILevel2 { }
        private interface ILevel1 : ILevel2 { }
        private interface ILevel0 : ILevel1 { }

        [TestMethod]
        public void TestImplementsOneLevel()
        {
            Assert.IsTrue(typeof(ILevel1).Implements(typeof(ILevel2)));
        }

        [TestMethod]
        public void TestImplementsTwoLevels()
        {
            Assert.IsTrue(typeof(ILevel0).Implements(typeof(ILevel2)));
        }

        [TestMethod]
        public void TestTransposeBetweenTwoClasses()
        {
            ClassOne source = new ClassOne { Field1 = "Hello", Field2 = "World", Property1 = "Goodbye", Property2 = "Universe" };

            ClassTwo target = source.Transpose<ClassOne, ClassTwo>();

            Assert.AreEqual("Hello", target.Field1);
            Assert.AreEqual("Universe", target.Property2);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void TestTransposeFailsWhenFieldTypeMismatches()
        {
            ClassOne source = new ClassOne { Field1 = "Hello", Field2 = "World", Property1 = "Goodbye", Property2 = "Universe" };

            source.Transpose<ClassOne, ClassThree>();
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void TestTransposeFailsWhenPropertyTypeMismatches()
        {
            ClassOne source = new ClassOne { Field1 = "Hello", Field2 = "World", Property1 = "Goodbye", Property2 = "Universe" };

            source.Transpose<ClassOne, ClassFour>();
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void TestTransposeFailsWhenFieldTypeIsClass()
        {
            ClassFive source = new ClassFive();

            source.Transpose<ClassFive, ClassFive>();
        }
        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void TestTransposeFailsWhenPropertyTypeIsClass()
        {
            ClassSix source = new ClassSix();

            source.Transpose<ClassSix, ClassSix>();
        }

        [TestMethod]
        public void TestTransposeDoesNotFailWhenPresentedWithReadonlyProperty()
        {
            ClassSeven source = new ClassSeven();
            source.Transpose<ClassSeven, ClassSeven>();
        }

        [TestMethod]
        public void TestTransposeDoesNotFailWhenPresentedWithReadonlyField()
        {
            ClassEight source = new ClassEight();
            source.Transpose<ClassEight, ClassEight>();
        }

        public class ClassOne
        {
            public string Field1;
            public string Field2;
            public string Property1 { get; set; }
            public string Property2 { get; set; }
        }

        public class ClassTwo
        {
            public string Field1;
            public string Property2 { get; set; }
        }

        public class ClassThree
        {
            public int Field1;
        }

        public class ClassFour
        {
            public int Property1 { get; set; }
        }

        public class ClassFive
        {
            public Timer Field1;
        }

        public class ClassSix
        {
            public Timer Property1 { get; set; }
        }
        
        public class ClassSeven
        {
            public string ReadonlyString { get { return "ReadOnly"; } }
        }

        public class ClassEight
        {
            public readonly string ReadonlyString = "ReadOnly";
        }
    }
}
