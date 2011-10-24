using System;

namespace CLAP
{
    public abstract class ParametersValidationAttribute : Attribute, IValidation
    {
        public abstract IInfoValidator GetValidator();
        public abstract string Description { get; }
    }
}