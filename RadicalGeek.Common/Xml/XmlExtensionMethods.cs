using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using RadicalGeek.Common.Reflection;
using RadicalGeek.Common.Collections;

namespace RadicalGeek.Common.Xml
{
    public static class XmlExtensionMethods
    {
        public static T ToObject<T>(this string xml)
        {
            try
            {
                using (XmlReader xmlReader = new XmlTextReader(new StringReader(xml)))
                    return (T)serializerCache[typeof(T)].Deserialize(xmlReader);
            }
            catch
            {
                return default(T);
            }
        }

        private static readonly CacheList<Assembly, Type[]> extraTypesCache =
            new CacheList<Assembly, Type[]>(a => a.GetTypes().Where(t => !t.IsInterface && t.IsPublic && t.HasParameterlessConstructor() && !t.IsGenericTypeDefinition).ToArray());

        private static readonly CacheList<Type, XmlSerializer> serializerCache =
            new CacheList<Type, XmlSerializer>(t => new XmlSerializer(t, extraTypesCache[t.Assembly]));

        public static string ToXmlString<T>(this T obj)
        {
            string result = string.Empty;
            try
            {
                if (!obj.Equals(default(T)))
                    using (var stringWriter = new StringWriter(CultureInfo.InvariantCulture))
                    {
                        Type type = obj.GetType();
                        serializerCache[type].Serialize(stringWriter, obj);
                        result = stringWriter.ToString();
                    }
            }
            catch
            {
                try
                {
                    if (string.IsNullOrEmpty(result))
                        result = obj.Serialize();
                }
                catch
                {
                    return null;
                }
            }
            return result;
        }

        private static string Serialize(this object obj)
        {
            if (obj == null) return string.Empty;
            if (obj.GetType().IsValueType || obj.GetType().FullName == "System.String") return obj.ToString();
            string output = string.Empty;
            Type type = obj.GetType();
            PropertyInfo[] propertyInfos = type.GetProperties();
            foreach (PropertyInfo info in propertyInfos)
            {
                object value = info.GetValue(obj, BindingFlags.Public | BindingFlags.Instance, null, null, null);
                if (value != null)
                {
                    output += string.Format("<{0}>", info.Name);
                    try
                    {
                        output += value.Serialize();
                    }
                    catch (Exception ex)
                    {
                        output += ex.Message;
                    }
                    output += string.Format("</{0}>", info.Name);
                }
            }
            return output;
        }
    }
}
