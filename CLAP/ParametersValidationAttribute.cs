using System;
using System.Reflection;

namespace CLAP
{
    public abstract class ParametersValidationAttribute : Attribute, IValidation<ParameterInfo>
    {
        public abstract IInfoValidator<ParameterInfo> GetValidator();
        public abstract string Description { get; }
    }
}