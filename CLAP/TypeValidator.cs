﻿using System.Diagnostics;
using System.Linq;
using System.Reflection;
using CLAP.Validation;

namespace CLAP
{
    internal static class TypeValidator
    {
        public static void Validate(object obj)
        {
            if (Ignore(obj))
            {
                return;
            }

            Debug.Assert(obj != null);

            var type = obj.GetType();

            // type's validators:
            // first use all the validators defined over this type
            //
            var validators = type.
                GetAttributes<ValidateAttribute>().
                Cast<IValidation>().
                Select(a => a.GetValidator());

            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            var propsAndValues = properties.
                Select(p => new ValueInfo(p.Name, p.PropertyType, p.GetValue(obj, null))).
                ToArray();

            foreach (var validator in validators)
            {
                validator.Validate(propsAndValues);
            }

            // property validators:
            // validate each property value, in case property is a custom class
            //
            foreach (var property in properties)
            {
                var value = property.GetValue(obj, null);

                if (Ignore(value))
                {
                    continue;
                }

                var propertyValidators = property.
                    GetAttributes<ValidateAttribute>().
                    Cast<IValidation>().
                    Select(a => a.GetValidator());

                foreach (var propertyValidator in propertyValidators)
                {
                    var propertyPropsAndValues = value.GetType().
                        GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).
                        Select(p => new ValueInfo(p.Name, p.PropertyType, p.GetValue(value, null))).
                        ToArray();

                    propertyValidator.Validate(propertyPropsAndValues);
                }
            }

            // recursion:
            // validate all values, in case their type has additional validation
            //
            foreach (var value in propsAndValues.Select(p => p.Value))
            {
                Validate(value);
            }
        }

        private static bool Ignore(object obj)
        {
            if (obj == null ||
                obj is string)
            {
                return true;
            }

            var type = obj.GetType();

            if (type.IsArray ||
                type.IsEnum ||
                type.Assembly.GlobalAssemblyCache)
            {
                return true;
            }


            return false;
        }
    }
}