using System;
using System.IO;
using System.Xml.Serialization;
using Newtonsoft.Json;

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
            else if (str.StartsWith(new[] { "{", "[" }))
            {
                obj = DeserializeJson(str, type);

                return true;
            }

            return false;
        }

        private static object DeserializeJson(string json, Type type)
        {
            return JsonConvert.DeserializeObject(json, type);
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