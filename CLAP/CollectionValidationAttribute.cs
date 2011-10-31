using System;

namespace CLAP
{
    /// <summary>
    /// Validates a collection of parameters of properties
    /// </summary>
    public abstract class CollectionValidationAttribute : Attribute, ICollectionValidation
    {
        public abstract ICollectionValidator GetValidator();
        public abstract string Description { get; }
    }
}