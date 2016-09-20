using System;
using System.Reflection;

namespace CLAP
{
#if NET20 || NET452
    internal static class TypeExtensions 
    {
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
    }
#else
    using System.Linq;
    internal static class TypeExtensions 
    {
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
    }
#endif
}