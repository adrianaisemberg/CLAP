using System;
using System.Collections.Generic;
using System.Reflection;

namespace CLAP
{
    /// <summary>
    /// Base exception class for all parser exceptions
    /// </summary>
    [Serializable]
    public abstract class CommandLineParserException : Exception
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public CommandLineParserException() { }

        /// <summary>
        /// Constructor
        /// </summary>
        public CommandLineParserException(string message) : base(message) { }

        /// <summary>
        /// Constructor
        /// </summary>
        public CommandLineParserException(string message, Exception inner) : base(message, inner) { }

        protected CommandLineParserException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class MissingDefaultVerbException : CommandLineParserException
    {
        public MissingDefaultVerbException()
            : this("No default verb was found")
        {
        }
        public MissingDefaultVerbException(string message) : base(message) { }
        public MissingDefaultVerbException(string message, Exception inner) : base(message, inner) { }
        protected MissingDefaultVerbException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class VerbNotFoundException : CommandLineParserException
    {
        /// <summary>
        /// The verb string that wasn't found
        /// </summary>
        public string Verb { get; private set; }

        public VerbNotFoundException(string verb)
            : base("Verb '{0}' was not found".FormatWith(verb))
        {
            Verb = verb;
        }

        public VerbNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected VerbNotFoundException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class MissingVerbException : CommandLineParserException
    {
        public MissingVerbException()
            : this("Arguments contain no verb", null)
        {
        }

        public MissingVerbException(string message, Exception inner) : base(message, inner) { }
        protected MissingVerbException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class MultiParserMissingClassNameException : CommandLineParserException
    {
        public MultiParserMissingClassNameException()
            : this("Arguments contain no class name nor verb", null)
        {
        }

        public MultiParserMissingClassNameException(string message, Exception inner) : base(message, inner) { }
        protected MultiParserMissingClassNameException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class InvalidVerbException : CommandLineParserException
    {
        public InvalidVerbException()
            : this("Invalid verb", null)
        {
        }

        public InvalidVerbException(string message, Exception inner) : base(message, inner) { }
        protected InvalidVerbException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class UnknownParserTypeException : CommandLineParserException
    {
        public UnknownParserTypeException(string typeName)
            : this("Parser type '{0}' not found".FormatWith(typeName), null)
        {
        }

        public UnknownParserTypeException(string message, Exception inner) : base(message, inner) { }
        protected UnknownParserTypeException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class MissingRequiredArgumentException : CommandLineParserException
    {
        /// <summary>
        /// The name of the required parameter
        /// </summary>
        public string ParameterName { get; private set; }

        /// <summary>
        /// The verb that requires the parameter
        /// </summary>
        public Method Method { get; private set; }

        public MissingRequiredArgumentException(Method method, string parameter)
            : base("Missing argument for required parameter '{0}'".FormatWith(parameter))
        {
            Method = method;
            ParameterName = parameter;
        }

        public MissingRequiredArgumentException(string message, Exception inner) : base(message, inner) { }
        protected MissingRequiredArgumentException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class MissingArgumentValueException : CommandLineParserException
    {
        /// <summary>
        /// The name of the parameter
        /// </summary>
        public string ParameterName { get; private set; }

        public MissingArgumentValueException(string parameter)
            : base("Parameter {0} has no input and is not a switch".FormatWith(parameter))
        {
            ParameterName = parameter;
        }

        public MissingArgumentValueException(string message, Exception inner) : base(message, inner) { }
        protected MissingArgumentValueException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class MissingArgumentPrefixException : CommandLineParserException
    {
        /// <summary>
        /// The name of the parameter
        /// </summary>
        public string ParameterName { get; private set; }

        public MissingArgumentPrefixException(string parameter, string prefixes)
            : base("'{0}' should be prefixed with one of '{1}'".FormatWith(parameter, prefixes))
        {
            ParameterName = parameter;
        }

        public MissingArgumentPrefixException(string message, Exception inner) : base(message, inner) { }
        protected MissingArgumentPrefixException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class ValidationException : CommandLineParserException
    {
        public ValidationException(string message) : base(message) { }
        public ValidationException(string message, Exception inner) : base(message, inner) { }
        protected ValidationException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class TypeConvertionException : CommandLineParserException
    {
        /// <summary>
        /// The string value that failed to be converted
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// The target type
        /// </summary>
        public Type Type { get; private set; }

        public TypeConvertionException(string value, Type type, Exception inner)
            : base("'{0}' cannot be converted to {1}".FormatWith(value, type), inner)
        {
            Value = value;
            Type = type;
        }

        protected TypeConvertionException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class MoreThanOneEmptyHandlerException : CommandLineParserException
    {
        public MoreThanOneEmptyHandlerException()
            : base("More than one empty handler was defined. Only a single method can be marked with [Empty] in a type and only a single action can be registered as an empty handler.")
        {
        }

        protected MoreThanOneEmptyHandlerException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class MoreThanOneErrorHandlerException : CommandLineParserException
    {
        public MoreThanOneErrorHandlerException()
            : base("More than one error handler was defined. Only a single method can be marked with [Error] in a type and only a single action can be registered as an error handler.")
        {
        }

        protected MoreThanOneErrorHandlerException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class MoreThanOnePreVerbInterceptorException : CommandLineParserException
    {
        public MoreThanOnePreVerbInterceptorException()
            : base("More than one pre-verb interceptor was defined. Only a single method can be marked with [PreVerbExecutionAttribute] in a type and only a single action can be registered as a pre-interceptor.")
        {
        }

        protected MoreThanOnePreVerbInterceptorException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class MoreThanOnePostVerbInterceptorException : CommandLineParserException
    {
        public MoreThanOnePostVerbInterceptorException()
            : base("More than one post-verb interceptor was defined. Only a single method can be marked with [PostVerbExecutionAttribute] in a type and only a single action can be registered as a post-interceptor.")
        {
        }

        protected MoreThanOnePostVerbInterceptorException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class ArgumentMismatchException : CommandLineParserException
    {
        public ArgumentMismatchException() { }
        public ArgumentMismatchException(string message) : base(message) { }
        public ArgumentMismatchException(string message, Exception inner) : base(message, inner) { }
        protected ArgumentMismatchException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class ParserExecutionTargetException : CommandLineParserException
    {
        public ParserExecutionTargetException() { }
        public ParserExecutionTargetException(string message) : base(message) { }
        public ParserExecutionTargetException(string message, Exception inner) : base(message, inner) { }
        protected ParserExecutionTargetException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class UnhandledParametersException : CommandLineParserException
    {
        /// <summary>
        /// The collection of unhandled arguments
        /// </summary>
        public Dictionary<string, string> UnhandledParameters { get; private set; }

        public UnhandledParametersException(Dictionary<string, string> unhandledParameters)
            : base("Unhandled parameters: '{0}'".FormatWith(unhandledParameters.Keys.StringJoin(", ")))
        {
            UnhandledParameters = unhandledParameters;
        }

        protected UnhandledParametersException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class MoreThanOneDefaultVerbException : CommandLineParserException
    {
        /// <summary>
        /// The collection of the verbs that are defined as default
        /// </summary>
        public IEnumerable<string> Verbs { get; private set; }

        public MoreThanOneDefaultVerbException(IEnumerable<string> verbs)
            : base("More than one default verb was defined: '{0}'".FormatWith(verbs.StringJoin(", ")))
        {
            Verbs = verbs;
        }

        protected MoreThanOneDefaultVerbException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class InvalidHelpHandlerException : CommandLineParserException
    {
        /// <summary>
        /// The method that is defined as help
        /// </summary>
        public MethodInfo Method { get; private set; }

        public InvalidHelpHandlerException(MethodInfo method)
            : this(method, null)
        {
        }

        public InvalidHelpHandlerException(MethodInfo method, Exception ex)
            : base("Method '{0}' is marked as [Help] but it does not have a single string parameter".FormatWith(method.Name), ex)
        {
            Method = method;
        }

        protected InvalidHelpHandlerException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class AmbiguousParameterDefaultException : CommandLineParserException
    {
        /// <summary>
        /// The parameter that has both a Default and a DefaultProvider
        /// </summary>
        public ParameterInfo Parameter { get; private set; }

        public AmbiguousParameterDefaultException(ParameterInfo parameter)
            : base("Parameter '{0}' of '{1}' has both a Default and a DefaultProvider".FormatWith(parameter.Name, parameter.Member.Name))
        {
            Parameter = parameter;
        }

        protected AmbiguousParameterDefaultException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class InvalidParameterDefaultProviderException : CommandLineParserException
    {
        /// <summary>
        /// The parameter that has an invalid DefaultProvider
        /// </summary>
        public ParameterInfo Parameter { get; private set; }

        public InvalidParameterDefaultProviderException(ParameterInfo parameter)
            : base("Parameter '{0}' of '{1}' has an invalid DefaultProvider".FormatWith(parameter.Name, parameter.Member.Name))
        {
            Parameter = parameter;
        }

        protected InvalidParameterDefaultProviderException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class DuplicateGlobalHandlerException : CommandLineParserException
    {
        /// <summary>
        /// The global handler name
        /// </summary>
        public string Name { get; private set; }

        public DuplicateGlobalHandlerException(string name)
            : base("Global parameter '{0}' is defined more than once".FormatWith(name))
        {
            Name = name;
        }

        protected DuplicateGlobalHandlerException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class NonArrayParameterWithSeparatorException : CommandLineParserException
    {
        /// <summary>
        /// The parameter
        /// </summary>
        public ParameterInfo Parameter { get; private set; }

        public NonArrayParameterWithSeparatorException(ParameterInfo parameter)
            : base("Parameter '{0}' of '{1}' is marked with [{2}] but it is not an array".FormatWith(
                parameter.Name,
                parameter.Member.Name,
                typeof(SeparatorAttribute).Name))
        {
            Parameter = parameter;
        }

        protected NonArrayParameterWithSeparatorException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class InvalidSeparatorException : CommandLineParserException
    {
        /// <summary>
        /// The parameter
        /// </summary>
        public ParameterInfo Parameter { get; private set; }

        public InvalidSeparatorException(ParameterInfo parameter)
            : base("Parameter '{0}' of '{1}' has an invalid separator. A separator cannot be empty or contain spaces.".FormatWith(
                parameter.Name,
                parameter.Member.Name))
        {
            Parameter = parameter;
        }

        protected InvalidSeparatorException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}