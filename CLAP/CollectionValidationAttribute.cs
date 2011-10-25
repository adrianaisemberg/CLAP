using System;

namespace CLAP
{
    public abstract class CollectionValidationAttribute : Attribute, ICollectionValidation
    {
        public abstract ICollectionValidator GetValidator();
        public abstract string Description { get; }
    }
}