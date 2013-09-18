using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RadicalGeek.Common.Patterns.InversionOfControl;
using RadicalGeek.Common.Patterns.Singleton;

namespace RadicalGeek.Common.UnitTests
{
    [TestClass]
    public class IoCTests
    {
        public interface IExist { }
        public interface IObj : IExist { }
        public abstract class ObjBase : IObj { }
        public class MyObj : ObjBase { }
        public interface INotImplemented { }
        public interface IOnce { }
        public class Once1 : IOnce
        {
            public Once1() { }
            public Once1(int a, ObjBase b, IObj c, MyObj d, object e) { }
        }
        public interface ITwice { }
        public class Twice1 : ITwice { }
        public class Twice2 : ITwice { }

        [TestMethod]
        public void ResolveOneImplementationSuccessfully()
        {
            IOnce resolved = IoC.Resolve<IOnce>();
            Assert.IsTrue(resolved is Once1);
        }

        [TestMethod]
        public void ResolveTypeImplementationSuccessfully()
        {
            Type resolved = IoC.ResolveType(typeof(IOnce));
            Assert.IsTrue(resolved == typeof(Once1));
        }

        [TestMethod]
        public void ResolveTypeGenericImplementationSuccessfully()
        {
            Type resolved = IoC.ResolveType<IOnce>();
            Assert.IsTrue(resolved == typeof(Once1));
        }

        [TestMethod]
        public void ConstructorValidationWorks()
        {
            MyObj objectInstance1 = new MyObj();
            MyObj objectInstance2 = SingletonFactory.Get<MyObj>();
            MyObj objectInstance3 = Activator.CreateInstance<MyObj>();
            IOnce anotherObject = IoC.Resolve<IOnce>();
            IoC.Resolve<IOnce>(1, objectInstance1, objectInstance2, objectInstance3, anotherObject);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void FailToResolveNotImplementedInterface()
        {
            IoC.Resolve<INotImplemented>();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void FailToResolveTwiceImplemented()
        {
            IoC.Resolve<ITwice>();
        }

        [TestMethod]
        public void ResolveIgnoresAbstractClasses()
        {
            Assert.IsTrue(IoC.Resolve<IObj>() is MyObj);
        }

        [TestMethod]
        public void ResolveIgnoresInterfaces()
        {
            Assert.IsTrue(IoC.Resolve<IExist>() is MyObj);
        }

        [TestMethod]
        public void ResolveReturnsCurrentOfSingleton()
        {
            
        }
    }
}