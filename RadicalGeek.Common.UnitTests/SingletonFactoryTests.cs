using Microsoft.VisualStudio.TestTools.UnitTesting;
using RadicalGeek.Common.Patterns.Singleton;

namespace RadicalGeek.Common.UnitTests
{
    [TestClass]
    public class SingletonFactoryTests
    {
        [TestMethod]
        public void SingletonFactoryConstructsWithPrivateConstructor()
        {
            MySingletonClass instance = SingletonFactory.Get<MySingletonClass>();
            Assert.IsTrue(instance.ConstructorCalled);
        }
        
        [TestMethod]
        public void SingletonFactoryConstructsWithPublicConstructor()
        {
            MyNormalSingletonClass instance = SingletonFactory.Get<MyNormalSingletonClass>();
            Assert.IsTrue(instance.ConstructorCalled);
        }

        public class MySingletonClass
        {
            public bool ConstructorCalled { get; private set; }

            private MySingletonClass()
            {
                ConstructorCalled = true;
            }
        } 
        
        public class MyNormalSingletonClass
        {
            public bool ConstructorCalled { get; private set; }

            public MyNormalSingletonClass()
            {
                ConstructorCalled = true;
            }
        }
    }
}
