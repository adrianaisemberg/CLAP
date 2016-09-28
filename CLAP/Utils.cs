using System;
using System.Collections.Generic;
using System.Reflection;

#if !NET20
using System.Linq;
#endif

namespace CLAP
{
    internal static class Utils
    {
        private static IEnumerable<Attribute> GetCustomAttributes<T>(MethodInfo method) where T : Attribute
        {
#if NETSTANDARD1_6
            return method.GetCustomAttributes<T>();
#else
            return Attribute.GetCustomAttributes(method, typeof(T));
#endif
        }
        private static Attribute GetCustomAttribute<T>(MethodInfo method) where T : Attribute
        {
#if NETSTANDARD1_6
            return method.GetCustomAttribute<T>();
#else
            return Attribute.GetCustomAttribute(method, typeof(T));
#endif
        }
        private static Attribute GetCustomAttribute<T>(Type type) where T : Attribute
        {
#if NETSTANDARD1_6
            return type.GetTypeInfo().GetCustomAttribute<T>();
#else
            return Attribute.GetCustomAttribute(type, typeof(T));
#endif
        }
        private static IEnumerable<Attribute> GetCustomAttributes<T>(Type type) where T : Attribute
        {
#if NETSTANDARD1_6
            return type.GetTypeInfo().GetCustomAttributes<T>();
#else
            return Attribute.GetCustomAttributes(type, typeof(T));
#endif
        }
        private static Attribute GetCustomAttribute<T>(ParameterInfo parameter) where T : Attribute 
        {
#if NETSTANDARD1_6
            return parameter.GetCustomAttribute<T>();
#else
            return Attribute.GetCustomAttribute(parameter, typeof(T));
#endif
        }
        private static IEnumerable<Attribute> GetCustomAttributes<T>(ParameterInfo parameter) where T : Attribute 
        {
#if NETSTANDARD1_6
            return parameter.GetCustomAttributes<T>();
#else
            return Attribute.GetCustomAttributes(parameter, typeof(T));
#endif
        }
        private static IEnumerable<Attribute> GetCustomAttributes<T>(MemberInfo member) where T : Attribute 
        {
#if NETSTANDARD1_6
            return member.GetCustomAttributes<T>();
#else
            return Attribute.GetCustomAttributes(member, typeof(T));
#endif
        }

        public static string FormatWith(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        public static T GetAttribute<T>(this MethodInfo method) where T : Attribute
        {

            var att = GetCustomAttribute<T>(method);

            return (T)att;
        }

        public static T GetAttribute<T>(this Type type) where T : Attribute
        {
            var att = GetCustomAttribute<T>(type);

            return (T)att;
        }

        public static T GetAttribute<T>(this ParameterInfo parameter) where T : Attribute
        {
            var att = GetCustomAttribute<T>(parameter);

            return (T)att;
        }

        public static IEnumerable<T> GetAttributes<T>(this ParameterInfo parameter) where T : Attribute
        {
            var atts = GetCustomAttributes<T>(parameter).Cast<T>();

            return atts;
        }

        public static IEnumerable<T> GetAttributes<T>(this PropertyInfo property) where T : Attribute
        {
            var atts = GetCustomAttributes<T>(property).Cast<T>();

            return atts;
        }

        public static IEnumerable<T> GetInterfaceAttributes<T>(this MethodInfo method)
        {
#if NETSTANDARD1_6
            return method.GetCustomAttributes(true).
                Where(a => a.GetType().GetTypeInfo().GetInterfaces().Contains(typeof(T))).
                Cast<T>();
#else
            return method.GetCustomAttributes(true).
                Where(a => a.GetType().GetInterfaces().Contains(typeof(T))).
                Cast<T>();
#endif
        }

        public static IEnumerable<T> GetAttributes<T>(this Type type) where T : Attribute
        {
            var atts = GetCustomAttributes<T>(type).Cast<T>();

            return atts;
        }

        public static bool HasAttribute<T>(this MethodInfo method) where T : Attribute
        {
#if NETSTANDARD1_6
            return method.IsDefined(typeof(T));
#else
            return Attribute.IsDefined(method, typeof(T));
#endif
        }

        public static bool HasAttribute<T>(this Type type) where T : Attribute
        {
#if NETSTANDARD1_6
            return type.GetTypeInfo().IsDefined(typeof(T));
#else
            return Attribute.IsDefined(type, typeof(T));
#endif
        }

        public static bool HasAttribute<T>(this ParameterInfo parameter) where T : Attribute
        {
#if NETSTANDARD1_6
            return parameter.IsDefined(typeof(T));
#else
            return Attribute.IsDefined(parameter, typeof(T));
#endif
        }

        public static IEnumerable<MethodInfo> GetMethodsWith<T>(this Type type) where T : Attribute
        {
            var methods = GetAllMethods(type).
                Where(m => m.HasAttribute<T>());

            return methods;
        }

        public static IEnumerable<MethodInfo> GetAllMethods(this Type type)
        {
#if NETSTANDARD1_6
            var t = type.GetTypeInfo();
#else
            var t = type;
#endif
            var methods = t.GetMethods(
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance |
                BindingFlags.Static |
                BindingFlags.FlattenHierarchy);

            return methods;
        }

        public static bool None<T>(this IEnumerable<T> collection)
        {
            return collection == null || !collection.Any();
        }

        public static string StringJoin(this IEnumerable<string> strings, string separator)
        {
            return string.Join(separator, strings.ToArray());
        }

        public static IEnumerable<string> SplitBy(this string str, string separator)
        {
            return str.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static IEnumerable<string> CommaSplit(this string str)
        {
            return SplitBy(str, ",");
        }

        public static void Each<T>(this IEnumerable<T> collection, Action<T, int> action)
        {
            var index = 0;

            foreach (var item in collection)
            {
                action(item, index);

                index++;
            }
        }

        public static string ToSafeString(this object obj, string nullValue)
        {
            return obj == null ? nullValue : obj.ToString();
        }

        public static bool StartsWith(this string str, IEnumerable<string> values)
        {
            return values.Any(v => str.StartsWith(v));
        }

        public static bool Contains(this string str, IEnumerable<string> values)
        {
            return values.Any(v => str.Contains(v));
        }

        public static string GetGenericTypeName(this Type type)
        {
#if NETSTANDARD1_6
            var t = type.GetTypeInfo();
#else
            var t = type;
#endif
            if (!t.IsGenericType)
            {
                return type.Name;
            }

            var genericTypeName = type.GetGenericTypeDefinition().Name;

            genericTypeName = genericTypeName.Remove(genericTypeName.IndexOf('`'));

            var genericArgs = t.GetGenericArguments().
                Select(a => GetGenericTypeName(a)).
                StringJoin(",");

            return "{0}<{1}>".FormatWith(genericTypeName, genericArgs);
        }
    }
}