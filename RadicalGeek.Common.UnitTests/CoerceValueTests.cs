using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RadicalGeek.Common.Data;

namespace RadicalGeek.Common.UnitTests
{
    [TestClass]
    public class CoerceValueTests
    {
        [TestMethod]
        public void CoerceStringFromString()
        {
            Assert.AreEqual("Hello", "Hello".CoerceValue<string>());
        }

        [TestMethod]
        public void CoerceInt32FromString()
        {
            Assert.AreEqual(1234, "1234".CoerceValue<int>());
        }

        [TestMethod]
        public void CoerceInt32FromStringWithoutNumbers()
        {
            Assert.AreEqual(0, "Hello".CoerceValue<int>());
        }

        [TestMethod]
        public void CoerceInt32FromNull()
        {
            Assert.AreEqual(0, DataExtensionMethods.CoerceValue<int>(null));
        }
        
        [TestMethod]
        public void CoerceNullableInt32FromNull()
        {
            Assert.IsFalse(DataExtensionMethods.CoerceValue<int?>(null).HasValue);
        }

        [TestMethod]
        public void CoerceInt32FromDBNull()
        {
            Assert.AreEqual(0, DBNull.Value.CoerceValue<int>());
        }

        [TestMethod]
        public void CoerceInt16FromString()
        {
            Assert.AreEqual((short)1234, "1234".CoerceValue<short>());
        }

        [TestMethod]
        public void CoerceStringFromInt32()
        {
            Assert.AreEqual("1234", 1234.CoerceValue<string>());
        }

        [TestMethod]
        public void CoerceStringFromGuid()
        {
            Guid testValue = Guid.NewGuid();
            Assert.AreEqual(testValue.ToString(), testValue.CoerceValue<string>());
        }

        [TestMethod]
        public void CoerceInterfaceFromClass()
        {
            MySubclass candidate = new MySubclass();
            IInterface result = candidate.CoerceValue<IInterface>();
            Assert.AreNotEqual(null,result);
        }

        [TestMethod]
        public void CoerceBaseFromClass()
        {
            MySubclass candidate = new MySubclass();
            MyClass result = candidate.CoerceValue<MyClass>();
            Assert.AreNotEqual(null, result);
        }
        
        [TestMethod]
        public void CoerceInterfaceFromBase()
        {
            MyClass candidate = new MyClass();
            IInterface result = candidate.CoerceValue<IInterface>();
            Assert.AreNotEqual(null, result);
        }

        [TestMethod]
        public void CoerceNullableDateTimeFromDbNullValue()
        {
            DateTime? test = DBNull.Value.CoerceValue<DateTime?>();
            Assert.IsFalse(test.HasValue);
        }


        [TestMethod]
        public void FallBackOnTryParse()
        {
            "1.234arse".CoerceValue<decimal>();
        }


        private interface IInterface{}

        private class MyClass:IInterface{}

        private class MySubclass : MyClass{}
    }
}