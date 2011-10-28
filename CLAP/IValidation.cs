
namespace CLAP
{
    public interface ICollectionValidation
    {
        ICollectionValidator GetValidator();

        /// <summary>
        /// The description of this validation attribute, used when asking for help
        /// </summary>
        string Description { get; }
    }

    public interface ICollectionValidator
    {
        void Validate(ValueInfo[] properties);
    }
}