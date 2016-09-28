﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using CLAP.Interception;

#if !NET20
using System.Linq;
#endif

namespace CLAP
{
    internal static class ValuesFactory
    {
        internal static object GetValueForParameter(
            ParameterInfo parameter,
            Type parameterType,
            string inputKey,
            string stringValue)
        {
            // a string doesn't need convertion
            //
            if (parameterType == typeof(string) && stringValue != null)
            {
                return stringValue;
            }

            // in case of a switch - the default is true/false according to the switch
            //
            if (parameterType == typeof(Boolean) && stringValue == null)
            {
                return inputKey != null;
            }
            else
            {
                // try JSON/XML deserializing it
                //
                try
                {
                    object obj = null;

                    if (Serialization.Deserialize(stringValue, parameterType, ref obj))
                    {
                        TypeValidator.Validate(obj);

                        return obj;
                    }
                    else
                    {
                        // if can't deserialize - try converting it
                        //
                        return ConvertParameterValue(inputKey, stringValue, parameter, parameterType, null);
                    }
                }
                catch (ValidationException)
                {
                    // validation exceptions are good to throw out
                    //
                    throw;
                }
                catch (Exception ex)
                {
                    // tried deserialize but failed - try converting
                    //
                    return ConvertParameterValue(inputKey, stringValue, parameter, parameterType, ex);
                }
            }

            throw new MissingArgumentValueException(inputKey);
        }

        private static object ConvertParameterValue(
            string inputKey,
            string stringValue,
            ParameterInfo parameter,
            Type parameterType,
            Exception deserializationException)
        {
            try
            {
                // if array
                if (parameterType.IsArray)
                {
                    var separator = SeparatorAttribute.DefaultSeparator;

                    if (parameter != null && parameter.HasAttribute<SeparatorAttribute>())
                    {
                        separator = parameter.GetAttribute<SeparatorAttribute>().Separator;
                    }

                    var stringValues = stringValue.SplitBy(separator);

                    // The type of the array element
                    //
                    var type = parameterType.GetElementType();

                    // Create a generic instance of the ConvertToArray method
                    //
                    var convertToArrayMethod = typeof(ValuesFactory).GetMethod(
                            "ConvertToArray",
                            BindingFlags.NonPublic | BindingFlags.Static).
                        MakeGenericMethod(type);

                    // Run the array converter
                    //
                    return convertToArrayMethod.Invoke(null, new[] { stringValues });
                }
                // if there is an input value
                else if (stringValue != null)
                {
                    // convert the string value to the relevant parameter type
                    //
                    return ConvertString(stringValue, parameterType);
                }
            }
            catch
            {
                throw new TypeConvertionException(stringValue, parameterType, deserializationException);
            }

            throw new MissingArgumentValueException(inputKey);
        }

        private static object ConvertString(string value, Type type)
        {
            if (type.IsEnum())
            {
                return Enum.Parse(type, value);
            }
            else if (type == typeof(Guid))
            {
                return string.IsNullOrEmpty(value) ? (object)null : new Guid(value);
            }
            else if (type == typeof(Uri))
            {
                return string.IsNullOrEmpty(value) ? (object)null : new Uri(Environment.ExpandEnvironmentVariables(value));
            }
            else if (type.IsGenericType() && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (string.IsNullOrEmpty(value))
                {
                    return null;
                }
                else
                {
                    var converter = new NullableConverter(type);

                    return Convert.ChangeType(value, converter.UnderlyingType);
                }
            }
            else
            {
                return Convert.ChangeType(value, type);
            }
        }

        /// <summary>
        /// This method is called via reflection
        /// </summary>
        private static TConvert[] ConvertToArray<TConvert>(string[] values)
        {
            return values.Select(c => ConvertString(c, typeof(TConvert))).Cast<TConvert>().ToArray();
        }

        internal static ParameterAndValue[] CreateParameterValues(
            Method method,
            object target,
            Dictionary<string, string> inputArgs,
            IEnumerable<Parameter> list)
        {
            var parameters = new List<ParameterAndValue>();

            // inputArgs is getting emptied.
            // create a copy for the VerbExecutionContext
            //
            var inputArgsCopy = inputArgs.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            foreach (var p in list)
            {
                var parameterInfo = p.ParameterInfo;

                var names = p.Names;

                // according to the parameter names, try to find a match from the input
                //
                var inputKey = names.FirstOrDefault(n => inputArgs.ContainsKey(n));

                // the input value
                //
                string stringValue = null;

                // the actual value, converted to the relevant parameter type
                //
                object value = null;

                // if no input was found that matches this parameter
                //
                if (inputKey == null)
                {
                    if (p.Required)
                    {
                        throw new MissingRequiredArgumentException(method, parameterInfo.Name);
                    }

                    if (p.DefaultProvider != null)
                    {
                        value = p.DefaultProvider.GetDefault(new VerbExecutionContext(method, target, inputArgsCopy));
                    }
                    else
                    {
                        // the default is the value
                        //
                        value = p.Default;
                    }

                    // convert the default value, if different from parameter's value (guid, for example)
                    //
                    if (value is string && !(parameterInfo.ParameterType == typeof(string)))
                    {
                        value = GetValueForParameter(parameterInfo, parameterInfo.ParameterType, "{DEFAULT}", (string)value);
                    }
                }
                else
                {
                    stringValue = inputArgs[inputKey];

                    // remove it so later we'll see which ones were not handled
                    //
                    inputArgs.Remove(inputKey);
                }

                if (value == null && inputKey != null)
                {
                    value = GetValueForParameter(parameterInfo, parameterInfo.ParameterType, inputKey, stringValue);
                }

                // validate each parameter
                //
                if (value != null && parameterInfo.HasAttribute<ValidationAttribute>())
                {
                    var validators = parameterInfo.GetAttributes<ValidationAttribute>().Select(a => a.GetValidator());

                    // all validators must pass
                    //
                    foreach (var validator in validators)
                    {
                        validator.Validate(new ValueInfo(parameterInfo.Name, parameterInfo.ParameterType, value));
                    }
                }

                // we have a valid value - add it to the list of parameters
                //
                parameters.Add(new ParameterAndValue(p, value));
            }

            return parameters.ToArray();
        }
    }
}