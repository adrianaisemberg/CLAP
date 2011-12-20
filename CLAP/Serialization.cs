using System;
using System.IO;
using System.Xml.Serialization;

#if !FW2
using System.Web.Script.Serialization;
#endif

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
#if FW2
            else
            {
                return false;
            }
#else
            else if (str.StartsWith(new[] { "{", "[" }))
            {
                obj = DeserializeJson(str, type);

                return true;
            }

            return false;
#endif
        }

#if !FW2
        private static object DeserializeJson(string json, Type type)
        {
            var serializer = new JavaScriptSerializer();

            var method = serializer.GetType().
                GetMethod("Deserialize", new[] { typeof(string) }).
                MakeGenericMethod(type);

            var obj = method.Invoke(serializer, new[] { json });

            return obj;
        }
#endif

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