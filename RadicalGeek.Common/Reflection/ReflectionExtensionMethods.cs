using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using RadicalGeek.Common.Collections;

namespace RadicalGeek.Common.Reflection
{
    public static class ReflectionExtensionMethods
    {
        private static readonly CacheList<KeyValuePair<Type, string>, PropertyInfo> propertyCache = new CacheList<KeyValuePair<Type, string>, PropertyInfo>(kvp => kvp.Key.GetProperty(kvp.Value, BindingFlags.Public | BindingFlags.Instance), TimeSpan.MaxValue, true, 8000);
        private static readonly CacheList<KeyValuePair<Type, string>, FieldInfo> fieldCache = new CacheList<KeyValuePair<Type, string>, FieldInfo>(kvp => kvp.Key.GetField(kvp.Value, BindingFlags.Public | BindingFlags.Instance), TimeSpan.MaxValue, true, 8000);

        public static bool HasParameterlessConstructor(this Type type, bool includePrivate = false)
        {
            if (type == null) throw new ArgumentNullException("type");
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;
            if (includePrivate) bindingFlags |= BindingFlags.NonPublic;
            return type.GetConstructors(bindingFlags).Any(c => !c.GetParameters().Any());
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes"), SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object[])"), SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj", Justification = "It's the object we're operating on, so 'obj' is a good name for it.")]
        public static bool SetPropertyValue<T>(this object obj, PropertyInfo property, T value)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            if (property == null) throw new ArgumentNullException("property");
            if (!property.PropertyType.IsSubclassOf(typeof(T)))
            {
                Type declaringType = property.DeclaringType;
                throw new ArgumentException(String.Format("Cannot set value of Type {0} on Property {1}.{2} of Type {3}", typeof(T), declaringType != null ? declaringType.Name : "", property.Name, property.ReflectedType));
            }
            try
            {
                property.SetValue(obj, value, null);
                return true;
            }
            catch
            {
                return false;
            }
        }

        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj", Justification = "It's the object we're operating on, so 'obj' is a good name for it.")]
        public static bool SetFieldValue<T>(this object obj, string fieldName, T value)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            FieldInfo field = fieldCache[new KeyValuePair<Type, string>(obj.GetType(), fieldName)];
            return SetFieldValue(obj, field, value);
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes"), SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object[])"), SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj", Justification = "It's the object we're operating on, so 'obj' is a good name for it.")]
        public static bool SetFieldValue<T>(this object obj, FieldInfo field, T value)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            if (field == null) throw new ArgumentNullException("field");
            if (!field.FieldType.IsSubclassOf(typeof(T)))
            {
                Type declaringType = field.DeclaringType;
                throw new ArgumentException(String.Format("Cannot set value of Type {0} on Field {1}.{2} of Type {3}", typeof(T), declaringType != null ? declaringType.Name : "", field.Name, field.ReflectedType));
            }
            try
            {
                field.SetValue(obj, value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj", Justification = "It's the object we're operating on, so 'obj' is a good name for it.")]
        public static bool SetPropertyValue<T>(this object obj, string propertyName, T value)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            PropertyInfo property = propertyCache[new KeyValuePair<Type, string>(obj.GetType(), propertyName)];
            return SetPropertyValue(obj, property, value);
        }

        public static MemberInfo[] GetValueMembers(this Type type, params string[] memberNames)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (memberNames == null || memberNames.Length == 0)
                return type.GetProperties().Cast<MemberInfo>().Union(type.GetFields()).ToArray();
            return memberNames.Select(name => (MemberInfo)type.GetField(name) ?? type.GetProperty(name)).ToArray();
        }

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter"), SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        public static MemberInfo[] GetValueMembers<T>(this Type type, params string[] memberNames)
        {
            MemberInfo[] allPropertiesAndFields = type.GetValueMembers(memberNames);
            return
                allPropertiesAndFields.Where(
                    m =>
                    m is PropertyInfo
                        ? ((PropertyInfo)m).PropertyType == typeof(T)
                        : ((FieldInfo)m).FieldType == typeof(T)).ToArray();
        }

        public static Type GetReturnType(this MemberInfo member)
        {
            if (member == null) throw new ArgumentNullException("member");
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo)member).FieldType;
                case MemberTypes.Method:
                    return ((MethodInfo)member).ReturnType;
                case MemberTypes.Property:
                    return ((PropertyInfo)member).PropertyType;
                default:
                    return null;
            }
        }

        public static bool ReturnTypeIs(this MemberInfo member, Type type)
        {
            Type returnType = member.GetReturnType();
            return returnType != null && (returnType.IsSubclassOf(type) || returnType == type);
        }

        public static bool Implements(this Type type, Type @interface)
        {
            return type.GetInterfaces().Contains(@interface);
        }

        private static MethodInfo memberwiseClone;

        /// <summary>
        ///Returns a memberwise clone of an object by copying the fields and properties. Note that fields of reference types will not be cloned, so obj1.User would be the same object as obj2.User. This extension method exposes the protected MemberwiseClone method on object, which Microsoft only wanted a class to call from within itself. This makes it impossible to MemberwiseClone a data object generated from a web service reference without a partial implementation, or to MemberwiseClone an object from a Third Party library.
        /// </summary>
        /// <returns>Returns a memberwise clone of the object</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Memberwise")]
        public static T MemberwiseClone<T>(this T source) where T : class
        {
            if (source == null) return null;
            if (memberwiseClone == null)
                memberwiseClone = typeof(object)
                    .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                    .First(mi => mi.Name == "MemberwiseClone" && mi.GetParameters().Length == 0);
            return (T)memberwiseClone.Invoke(source, null);
        }

        private static IEnumerable<string> assemblySearchPaths;

        public static IEnumerable<Type> GetAllTypes(this Assembly assembly)
        {
            IEnumerable<Assembly> loadedAssemblies =
                new[]
                    {
                        Assembly.GetCallingAssembly(), Assembly.GetEntryAssembly(), Assembly.GetExecutingAssembly()
                    }.Distinct().Where(a => a != null);
            assemblySearchPaths = loadedAssemblies.Select(a => Path.GetDirectoryName(a.Location)).ToArray();
            IEnumerable<string> assemblyFiles = assemblySearchPaths.SelectMany(p => Directory.GetFiles(p, "*.exe")).Union(assemblySearchPaths.SelectMany(p => Directory.GetFiles(p, "*.dll")));
            IEnumerable<Assembly> assemblies = assemblyFiles.Select(f => f.LoadAssembly()).Where(a => a != null).Distinct();
            List<Type> types = new List<Type>();
            foreach (Assembly a in assemblies)
                try
                {
                    types.AddRange(a.GetTypes());
                }
                catch (ReflectionTypeLoadException e)
                {
                    types.AddRange(e.Types.Where(t => t != null));
                }
            return types.ToArray();
        }

        [SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Reflection.Assembly.LoadFrom", Justification = "The whole point of the method is to call Assembly.LoadFrom."),
 SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Generally we don't care why the assembly couldn't be loaded.")]
        public static Assembly LoadAssembly(this string f)
        {
            try
            {
                return Assembly.LoadFrom(f);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static T GetCustomAttribute<T>(this MemberInfo member) where T : Attribute
        {
            return (T)member.GetCustomAttributes(false).FirstOrDefault(a => a.GetType() == typeof(T));
        }

        public static void SetValue(this PropertyInfo propertyInfo, object obj, object value)
        {
            propertyInfo.SetValue(obj, value, null);
        }

        public static object GetValue(this PropertyInfo propertyInfo, object obj)
        {
            return propertyInfo.GetValue(obj, null);
        }

        public static TOut Transpose<TIn, TOut>(this TIn input)
        {
            Type outType = typeof(TOut);
            Type inType = typeof(TIn);

            TOut result;

            if (!outType.IsValueType && outType.HasParameterlessConstructor())
                result = Activator.CreateInstance<TOut>();
            else
                result = default(TOut);

            FieldInfo[] fields = inType.GetFields();
            IEnumerable<Tuple<FieldInfo, FieldInfo>> fieldInfos =
                fields.Select(f => new Tuple<FieldInfo, FieldInfo>(f, outType.GetField(f.Name)));
            foreach (Tuple<FieldInfo, FieldInfo> tuple in fieldInfos.Where(t => t.Item2 != null))
            {
                FieldInfo sourceField = tuple.Item1;
                FieldInfo targetField = tuple.Item2;

                if (!sourceField.FieldType.IsValueType && sourceField.FieldType != typeof(string))
                    throw new InvalidOperationException(string.Format("Cannot transpose from field {0} of type {2} because it is of type {1} which is not a value type", sourceField.Name, sourceField.FieldType.Name, inType.Name));
                if (!targetField.FieldType.IsValueType && targetField.FieldType != typeof(string))
                    throw new InvalidOperationException(string.Format("Cannot transpose to field {0} of type {2} because it is of type {1} which is not a value type", targetField.Name, targetField.FieldType.Name, outType.Name));
                if (sourceField.FieldType != targetField.FieldType)
                    throw new InvalidOperationException(string.Format("Cannot transpose field {0} because in type {1} it is of type {2} and in type {3} it is of type {4}", sourceField.Name, inType.Name, sourceField.FieldType.Name, outType.Name, targetField.FieldType.Name));

                object value = sourceField.GetValue(input);
                targetField.SetValue(result, value);
            }

            PropertyInfo[] properties = inType.GetProperties();
            IEnumerable<Tuple<PropertyInfo, PropertyInfo>> propertyInfos =
                properties.Select(p => new Tuple<PropertyInfo, PropertyInfo>(p, outType.GetProperty(p.Name)));
            foreach (Tuple<PropertyInfo, PropertyInfo> tuple in propertyInfos.Where(t => t.Item2 != null && t.Item2.GetSetMethod() != null))
            {
                PropertyInfo sourceProperty = tuple.Item1;
                PropertyInfo targetProperty = tuple.Item2;

                if (!sourceProperty.PropertyType.IsValueType && sourceProperty.PropertyType != typeof(string))
                    throw new InvalidOperationException(string.Format("Cannot transpose from property {0} of type {2} because it is of type {1} which is not a value type", sourceProperty.Name, sourceProperty.PropertyType.Name, inType.Name));
                if (!targetProperty.PropertyType.IsValueType && targetProperty.PropertyType != typeof(string))
                    throw new InvalidOperationException(string.Format("Cannot transpose to property {0} of type {2} because it is of type {1} which is not a value type", targetProperty.Name, targetProperty.PropertyType.Name, outType.Name));
                if (sourceProperty.PropertyType != targetProperty.PropertyType)
                    throw new InvalidOperationException(string.Format("Cannot transpose property {0} because in type {1} it is of type {2} and in type {3} it is of type {4}", sourceProperty.Name, inType.Name, sourceProperty.PropertyType.Name, outType.Name, targetProperty.PropertyType.Name));

                object value = tuple.Item1.GetValue(input);
                tuple.Item2.SetValue(result, value);
            }

            return result;
        }

        public static void MergeInto<T>(this T source, T target)
        {
            Type type = typeof(T);

            IEnumerable<FieldInfo> fields = type.GetFields().Where(f => f.GetCustomAttribute<MergeFieldAttribute>() != null);
            foreach (FieldInfo field in fields)
            {
                object value = field.GetValue(source);
                field.SetValue(target, value);
            }

            IEnumerable<PropertyInfo> properties = type.GetProperties().Where(f => f.GetCustomAttribute<MergeFieldAttribute>() != null);
            foreach (PropertyInfo property in properties)
            {
                object value = property.GetValue(source);
                property.SetValue(target, value);
            }
        }
    }
}