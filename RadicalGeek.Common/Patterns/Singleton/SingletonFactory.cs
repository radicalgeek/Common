using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RadicalGeek.Common.Reflection;

namespace RadicalGeek.Common.Patterns.Singleton
{
    public static class SingletonFactory
    {
        private static readonly KeyedByTypeCollection<object> instanceDictionary = new KeyedByTypeCollection<object>();

        public static T Get<T>() where T : class
        {
            return (T)Get(typeof(T));
        }

        public static object Get(Type t)
        {
            if (!t.IsClass && !t.HasParameterlessConstructor(true))
                throw new InvalidOperationException("The type must be a class with a parameterless constructor to be used as a Singleton");
            lock (instanceDictionary)
            {
                if (!instanceDictionary.Contains(t))
                    instanceDictionary.Add(CreateInstance(t));
                return instanceDictionary[t];
            }
        }

        private static object CreateInstance(Type t)
        {
            return t.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).First(c => !c.GetParameters().Any()).Invoke(new object[0]);
        }
    }
}
