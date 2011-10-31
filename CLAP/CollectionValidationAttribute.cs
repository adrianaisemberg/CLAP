using System;

namespace CLAP
{
    /// <summary>
    /// Validates a collection of parameters of properties
    /// </summary>
    public abstract class CollectionValidationAttribute : Attribute, ICollectionValidation
    {
        /// <summary>
        /// Gets a validator instance
        /// </summary>
        public abstract ICollectionValidator GetValidator();

        /// <summary>
        /// The validation description
        /// </summary>
        public abstract string Description { get; }
    }
}