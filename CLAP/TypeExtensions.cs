using System;
using System.Reflection;

namespace CLAP
{
#if NET20 || NET452
    internal static class TypeExtensions 
    {
        public static Assembly Assembly(this Type type)
        {
            return type.Assembly;
        } 
        public static bool GlobalAssemblyCache(this Assembly assembly)
        {
            return assembly.GlobalAssemblyCache;
        } 
        public static bool IsEnum(this Type type)
        {
            return type.IsEnum;
        } 
        public static bool IsGenericType(this Type type)
        {
            return type.IsGenericType;
        } 
        public static object[] GetCustomAttributes(this Type type, Type attributeType, bool inherit)
        {
            return type.GetCustomAttributes(attributeType, inherit);
        } 
        public static Type[] GetGenericArguments(this Type type)
        {
            return type.GetGenericArguments();
        } 
        public static bool IsArray(this Type type)
        {
            return type.IsArray;
        } 
    }
#else
    using System.Linq;
    internal static class TypeExtensions 
    {
        public static Assembly Assembly(this Type type)
        {
            return type.GetTypeInfo().Assembly;
        } 
        public static bool GlobalAssemblyCache(this Assembly assembly)
        {
            return false;
        } 
        public static bool IsEnum(this Type type)
        {
            return type.GetTypeInfo().IsEnum;
        } 
        public static bool IsGenericType(this Type type)
        {
            return type.GetTypeInfo().IsGenericType;
        } 
        public static object[] GetCustomAttributes(this Type type, Type attributeType, bool inherit)
        {
            return type.GetTypeInfo().GetCustomAttributes<Attribute>(inherit)
                .Where(i => i.GetType() == attributeType)
                .Cast<object>()
                .ToArray();
        } 
        public static Type[] GetGenericArguments(this Type type)
        {
            return type.GetTypeInfo().GetGenericArguments();
        } 
        public static bool IsArray(this Type type)
        {
            return type.IsArray;
        } 
    }
#endif
}