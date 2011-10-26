using System;
using System.Collections.Generic;

namespace CLAP
{
    [Serializable]
    public abstract class CommandLineException : Exception
    {
        public CommandLineException() { }
        public CommandLineException(string message) : base(message) { }
        public CommandLineException(string message, Exception inner) : base(message, inner) { }
        protected CommandLineException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public abstract class ParserException : Exception
    {
        public ParserException() { }
        public ParserException(string message) : base(message) { }
        public ParserException(string message, Exception inner) : base(message, inner) { }
        protected ParserException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class MissingDefaultVerbException : CommandLineException
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
    public class MissingVerbException : CommandLineException
    {
        public string Verb { get; private set; }

        public MissingVerbException(string verb)
            : base("Verb '{0}' was not found".FormatWith(verb))
        {
            Verb = verb;
        }

        public MissingVerbException(string message, Exception inner) : base(message, inner) { }
        protected MissingVerbException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class MissingRequiredArgumentException : CommandLineException
    {
        public string ParameterName { get; private set; }
        public string Verb { get; private set; }

        public MissingRequiredArgumentException(string verb, string parameter)
            : base("Missing argument for required parameter '{0}'".FormatWith(parameter))
        {
            Verb = verb;
            ParameterName = parameter;
        }

        public MissingRequiredArgumentException(string message, Exception inner) : base(message, inner) { }
        protected MissingRequiredArgumentException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class MissingArgumentValueException : CommandLineException
    {
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
    public class MissingArgumentPrefixException : CommandLineException
    {
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
    public class ValidationException : CommandLineException
    {
        public ValidationException(string message) : base(message) { }
        public ValidationException(string message, Exception inner) : base(message, inner) { }
        protected ValidationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class TypeConvertionException : CommandLineException
    {
        public string Value { get; private set; }
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
    public class MoreThanOneEmptyHandlerException : CommandLineException
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
    public class MoreThanOneErrorHandlerException : CommandLineException
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
    public class MoreThanOnePreVerbInterceptorException : CommandLineException
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
    public class MoreThanOnePostVerbInterceptorException : CommandLineException
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
    public class ArgumentMismatchException : CommandLineException
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
    public class ParserExecutionTargetException : ParserException
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
    public class UnhandledParametersException : CommandLineException
    {
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
    public class MoreThanOneDefaultVerbException : CommandLineException
    {
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

}