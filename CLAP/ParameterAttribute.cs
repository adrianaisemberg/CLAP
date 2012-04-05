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

    [Serializable]
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class RequiredAttribute : Attribute
    {
    }

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

    [Serializable]
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class DescriptionAttribute : Attribute
    {
        public string Description { get; private set; }

        public DescriptionAttribute(string description)
        {
            Description = description;
        }
    }

    [Serializable]
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class SeparatorAttribute : Attribute
    {
        public string Separator { get; private set; }

        public SeparatorAttribute(string separator)
        {
            Separator = separator;
        }
    }
}