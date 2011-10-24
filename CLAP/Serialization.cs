using System;
using System.Web.Script.Serialization;

namespace CLAP
{
    internal static class Serialization
    {
        public static object Deserialize(string json, Type type)
        {
            var serializer = new JavaScriptSerializer();

            var method = serializer.GetType().
                GetMethod("Deserialize", new[] { typeof(string) }).
                MakeGenericMethod(type);

            var obj = method.Invoke(serializer, new[] { json });

            return obj;
        }
    }
}