using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reflection;
using RadicalGeek.Common.Collections;

namespace RadicalGeek.Common.Data
{
    public static class DataExtensionMethods
    {
        public static void PopulateFromDataReader<T>(this T obj, IDataReader reader) where T : class, new()
        {
            obj.PopulateFromDataReader(reader, true);
        }

        private static readonly CacheList<KeyValuePair<Type, string>, PropertyInfo> propertyCache = new CacheList<KeyValuePair<Type, string>, PropertyInfo>(kvp => kvp.Key.GetProperty(kvp.Value, BindingFlags.Public | BindingFlags.Instance), TimeSpan.MaxValue, true, 8000);
        private static readonly CacheList<KeyValuePair<Type, string>, FieldInfo> fieldCache = new CacheList<KeyValuePair<Type, string>, FieldInfo>(kvp => kvp.Key.GetField(kvp.Value, BindingFlags.Public | BindingFlags.Instance), TimeSpan.MaxValue, true, 8000);

        public static ReadOnlyCollection<T> ExecuteToReadOnlyCollection<T>(this IDbCommand command) where T : class, new()
        {
            if (command == null) throw new ArgumentNullException("command");
            if (command.Connection == null) throw new InvalidOperationException("There is no Connection associated with the command object");
            List<T> result = new List<T>();
            using (IDataReader dataReader = command.ExecuteReader())
                while (dataReader.Read())
                    result.Add(dataReader.ToObject<T>());
            return new ReadOnlyCollection<T>(result);
        }

        public static DataTable ExecuteToDataTable(this IDbCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (command.Connection == null) throw new InvalidOperationException("There is no Connection associated with the command object");
            DataTable result = new DataTable();
            using (IDataReader dataReader = command.ExecuteReader())
                result.Load(dataReader);
            return result;
        }

        public static List<T> ExecuteToList<T>(this IDbCommand command) where T : class, new()
        {
            if (command == null) throw new ArgumentNullException("command");
            if (command.Connection == null) throw new InvalidOperationException("There is no Connection associated with the command object");
            using (IDataReader dataReader = command.ExecuteReader())
                return dataReader.ToList<T>();
        }

        public static List<T> ToList<T>(this IDataReader reader) where T : class,new()
        {
            List<T> result = new List<T>();
            while (reader.Read())
                result.Add(reader.ToObject<T>());
            return result;
        }

        public static List<TList> ToList<TList, TContents>(this IDataReader reader) where TContents : class, TList, new()
        {
            List<TList> result = new List<TList>();
            while (reader.Read())
                result.Add(reader.ToObject<TContents>());
            return result;
        }

        public static T ToObject<T>(this IDataReader reader) where T : class, new()
        {
            T result = new T();
            result.PopulateFromDataReader(reader, false);
            return result;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "initialise"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "PopulateFromDataReader")]
        public static void PopulateFromDataReader<T>(this T obj, IDataReader reader, bool tryRead) where T : class, new()
        {
            if (obj == null)
                throw new InvalidOperationException("Cannot call PopulateFromDataReader<T> on a null object - initialise it before populating.");
            if (!tryRead || reader.Read())
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string name = reader.GetName(i);
                    KeyValuePair<Type, string> key = new KeyValuePair<Type, string>(typeof(T), name);
                    object value = reader[i].IsDbNull() ? null : reader[i];
                    try
                    {
                        PropertyInfo propertyInfo = propertyCache[key];
                        if (propertyInfo != null)
                        {
                            propertyInfo.SetValue(obj, value, null);
                        }
                        else
                        {
                            FieldInfo fieldInfo = fieldCache[key];
                            if (fieldInfo != null)
                                fieldInfo.SetValue(obj, value);
                        }
                    }
                    catch (ArgumentException ex)
                    {
                        throw new InvalidOperationException(string.Format("Failed to populate property or field {0} with value of type {1}", name, value != null ? value.GetType().Name : "null"), ex);
                    }
                }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Db", Justification = "Conflicts with Microsoft's practices - e.g. IDbConnection - and Resharper's recommendations.")]
        public static bool IsDbNull(this object value)
        {
            return DBNull.Value.Equals(value);
        }

        static readonly CacheList<Type, MethodInfo> tryParseMethod =
            new CacheList<Type, MethodInfo>(
                t => t.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(m => m.Name == "TryParse" && m.GetParameters().Length == 2 && m.ReturnType == typeof(bool)));

        // ReSharper disable EmptyGeneralCatchClause
        public static T CoerceValue<T>(this object source)
        {
            if (source == null || DBNull.Value.Equals(source)) return default(T);
            if (source is T) return (T)source;
            if (source is DateTime && typeof(T) == typeof(string))
                source = (((DateTime)source).ToString("S"));
            try
            {
                return (T)Convert.ChangeType(source, typeof(T));
            }
            catch
            {
            }
            try
            {
                MethodInfo tryParse = tryParseMethod[typeof(T)];
                if (source is string && tryParse != null)
                {
                    T result = default(T);
                    bool success = (bool)tryParse.Invoke(null, new[] { source, result });
                    if (success)
                        return result;
                }
            }
            catch
            {
            }
            try
            {
                if (typeof(T) == typeof(string))
                    return (T)Convert.ChangeType(source.ToString(), typeof(T));
            }
            catch
            {
            }
            return default(T);
        }
        // ReSharper restore EmptyGeneralCatchClause
    }
}
