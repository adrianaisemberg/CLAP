using System;

namespace CLAP
{
    public abstract class CollectionValidationAttribute : Attribute, IValidation
    {
        public abstract IInfoValidator GetValidator();
        public abstract string Description { get; }
    }
}