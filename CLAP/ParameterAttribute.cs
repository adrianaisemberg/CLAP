using System;

namespace CLAP
{
    /// <summary>
    /// A parameter
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Parameter)]
    [Obsolete("Use DefaultValueAttribute, DefaultProviderAttribute, RequiredAttribute, AliasesAttribute and DescriptionAttribute")]
    public sealed class ParameterAttribute : Attribute
    {
        /// <summary>
        /// The default value
        /// </summary>
        [Obsolete("Use DefaultValueAttribute")]
        public object Default { get; set; }

        /// <summary>
        /// The default provider type
        /// </summary>
        /// <remarks>
        /// The type must derive from CLAP.DefaultProvider.
        /// A parameter cannot have both a Default and a DefaultProvider defined.
        /// </remarks>
        [Obsolete("Use DefaultProviderAttribute")]
        public Type DefaultProvider { get; set; }

        /// <summary>
        /// Whether this parameter is required
        /// </summary>
        [Obsolete("Use RequiredAttribute")]
        public Boolean Required { get; set; }

        /// <summary>
        /// The parameter additional names
        /// </summary>
        [Obsolete("Use AliasesAttribute")]
        public string Aliases { get; set; }

        /// <summary>
        /// The description of the verb. Used to generate the help string
        /// </summary>
        [Obsolete("Use DescriptionAttribute")]
        public string Description { get; set; }
    }

    /// <summary>
    /// Sets a default value for a parameter
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class DefaultValueAttribute : Attribute
    {
        public object DefaultValue { get; private set; }

        public DefaultValueAttribute(object defaultValue)
        {
            DefaultValue = defaultValue;
        }
    }

    /// <summary>
    /// Sets a default value provider type for a parameter.
    /// The type must derive from DefaultProvider
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class DefaultProviderAttribute : Attribute
    {
        public Type DefaultProviderType { get; private set; }

        public DefaultProviderAttribute(Type defaultProviderType)
        {
            DefaultProviderType = defaultProviderType;
        }
    }

    /// <summary>
    /// Marks a parameter to be required
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class RequiredAttribute : Attribute
    {
    }

    /// <summary>
    /// Sets additional names to a parameter
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class AliasesAttribute : Attribute
    {
        public string Aliases { get; private set; }

        public AliasesAttribute(string aliases)
        {
            Aliases = aliases;
        }
    }

    /// <summary>
    /// Sets a description to a parameter
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Parameter)]
    public class DescriptionAttribute : Attribute
    {
        public virtual string Description { get; private set; }

        public DescriptionAttribute(string description)
        {
            Description = description;
        }
    }

    /// <summary>
    /// Sets an array parameter separator
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class SeparatorAttribute : Attribute
    {
        public string Separator { get; private set; }

        public const string DefaultSeparator = ",";

        /// <summary>
        /// The separator to use.
        /// The default is a comma (",")
        /// </summary>
        /// <param name="separator"></param>
        public SeparatorAttribute(string separator)
        {
            Separator = separator;
        }
    }
}