using System;
using System.IO;
using System.Web.Script.Serialization;
using System.Xml.Serialization;

namespace CLAP
{
    internal static class Serialization
    {
        public static bool Deserialize(string str, Type type, ref object obj)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }

            if (str.StartsWith("<"))
            {
                obj = DeserializeXml(str, type);

                return true;
            }
            else if (str.StartsWith("{") || str.StartsWith("["))
            {
                obj = DeserializeJson(str, type);

                return true;
            }

            return false;
        }

        public static object DeserializeJson(string json, Type type)
        {
            var serializer = new JavaScriptSerializer();

            var method = serializer.GetType().
                GetMethod("Deserialize", new[] { typeof(string) }).
                MakeGenericMethod(type);

            var obj = method.Invoke(serializer, new[] { json });

            return obj;
        }

        public static object DeserializeXml(string xml, Type type)
        {
            var serializer = new XmlSerializer(type);

            using (var reader = new StringReader(xml))
            {
                var obj = serializer.Deserialize(reader);

                return obj;
            }
        }
    }
}