using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using RadicalGeek.Common.Reflection;
using RadicalGeek.Common.Collections;

namespace RadicalGeek.Common.Patterns.InversionOfControl
{
    /// <summary>
    /// Provides zero-configuration Inversion of Control methods
    /// </summary>
    public static class IoC
    {
        private static readonly AutoDictionary<Type, Type> resolutionCache = new AutoDictionary<Type, Type>();
        private static IEnumerable<Type> types;

        /// <summary>
        /// Searches the local assemblies for implementations of the given interface and returns the implementing Type if there is only one
        /// </summary>
        /// <param name="interfaceType">The interface type to find an implementation of</param>
        /// <returns>Returns the Type that implements the given interface, or throws an exception if there are zero or more than one implementation.</returns>
        public static Type ResolveType(Type interfaceType)
        {
            if (!interfaceType.IsInterface)
                throw new InvalidOperationException("You must pass an Interface to Resolve");

            if (!resolutionCache.ContainsKey(interfaceType))
            {
                types = types ?? Assembly.GetCallingAssembly().GetAllTypes();

                List<Type> implementations = new List<Type>(types.Where(t => !t.IsInterface && !t.IsAbstract && t.Implements(interfaceType)));

                if (implementations.Count > 1)
                    throw new InvalidOperationException(string.Format("Multiple implementations of {0} were found: {1}", interfaceType.Name, string.Join(", ", implementations.Select(t => string.Format("{0} in {1}", t.Name, Path.GetFileName(t.Assembly.Location))))));

                if (implementations.Count < 1)
                    throw new InvalidOperationException(string.Format("No implementations of {0} were found.", interfaceType.Name));

                resolutionCache.Add(interfaceType, implementations[0]);
            }

            return resolutionCache[interfaceType];
        }

        /// <summary>
        /// Searches the local assemblies for implementations of the given interface and returns the implementing Type if there is only one
        /// </summary>
        /// <typeparam name="TInterface">The interface type to find an implementation of</typeparam>
        /// <returns>Returns the Type that implements the given interface, or throws an exception if there are zero or many implementations.</returns>
        public static Type ResolveType<TInterface>() where TInterface : class
        {
            Type interfaceType = typeof(TInterface);
            return ResolveType(interfaceType);
        }

        /// <summary>
        /// Adds a resolution "shortcut" that bypasses the assembly search. It is recommended to only use this for unit testing, to inject specific mock or stub implementations.
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        public static void AddResolutionShortcut<TInterface, TImplementation>() where TImplementation : TInterface
        {
            AddResolutionShortcut(typeof(TInterface), typeof(TImplementation));
        }

        /// <summary>
        /// Adds a resolution "shortcut" that bypasses the assembly search. It is recommended to only use this for unit testing, to inject specific mock or stub implementations.
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <param name="implementationType"></param>
        public static void AddResolutionShortcut(Type interfaceType, Type implementationType)
        {
            resolutionCache[interfaceType] = implementationType;
        }

        /// <summary>
        /// Searches the local assemblies for implementations of the given interface and returns an instance of the implementing Type if there is only one
        /// </summary>
        /// <typeparam name="TInterface">The interface type to find an implementation of</typeparam>
        /// <returns>Returns an instance of the Type that implements the given interface, or throws an exception if there are zero or more than one implementation.</returns>
        public static TInterface Resolve<TInterface>(params object[] args) where TInterface : class
        {
            Type resolvedType = ResolveType<TInterface>();

            ValidateConstructor(typeof(TInterface), resolvedType, args);

            return (TInterface)Activator.CreateInstance(resolvedType, args);
        }

        private static readonly Func<Type, Type, bool>[] parameterValidators
            = new Func<Type, Type, bool>[]
                {
                    (tArg,tParam) => tArg == tParam,
                    (tArg,tParam) => tParam.IsInterface && tArg.Implements(tParam),
                    (tArg,tParam) => tArg != tParam && tArg.IsSubclassOf(tParam),
                    (tArg,tParam) => tArg != tParam && tParam.IsAssignableFrom(tArg)
                };

        private static void ValidateConstructor(Type interfaceType, Type resolvedType, IList<object> args)
        {
            ConstructorInfo[] allConstructors = resolvedType.GetConstructors();
            ConstructorInfo[] semiFinalists = allConstructors.Where(c => c.GetParameters().Length == args.Count).ToArray();
            if (semiFinalists == null || semiFinalists.Length == 0)
                throw new InvalidOperationException(
                    string.Format("{2} resolved to {1} which was passed {0} arguments, but no suitable constructors were found", args.Count,
                                  resolvedType.Name, interfaceType.Name));

            List<ConstructorInfo> finalists = new List<ConstructorInfo>();
            foreach (ConstructorInfo constructor in semiFinalists)
            {
                ParameterInfo[] parameters = constructor.GetParameters();
                int i = 0;
                int parameterMatches = 0;
                foreach (ParameterInfo parameter in parameters)
                {
                    int matches = parameterValidators.Count(v => v(args[i].GetType(),parameter.ParameterType));
                    if (matches >= 1)
                        parameterMatches++;
                    i++;
                }
                if (parameterMatches == parameters.Length && finalists.Count == 1)
                    throw new InvalidOperationException(string.Format("{1} resolved to {0} but no constructors matched the requested signature", resolvedType.Name, interfaceType.Name));
                finalists.Add(constructor);
            }
            if (finalists.Count == 0)
                throw new InvalidOperationException(string.Format("{1} resolved to {0} but multiple constructors match the requested signature", resolvedType.Name, interfaceType.Name));
        }
    }
}
