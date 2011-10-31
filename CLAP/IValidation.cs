
namespace CLAP
{
    /// <summary>
    /// Validation of collections of parameters and values
    /// </summary>
    public interface ICollectionValidation
    {
        /// <summary>
        /// Gets an instance of the collection validator
        /// </summary>
        /// <returns></returns>
        ICollectionValidator GetValidator();

        /// <summary>
        /// The description of this validation attribute, used when asking for help
        /// </summary>
        string Description { get; }
    }

    /// <summary>
    /// Validation of collections of parameters and values
    /// </summary>
    public interface ICollectionValidator
    {
        void Validate(ValueInfo[] properties);
    }
}