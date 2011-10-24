
namespace CLAP
{
    public interface IValidation<T>
    {
        IInfoValidator<T> GetValidator();

        /// <summary>
        /// The description of this validation attribute, used when asking for help
        /// </summary>
        string Description { get; }
    }

    public interface IInfoValidator<T>
    {
        void Validate(InfoAndValue<T>[] properties);
    }

    public class InfoAndValue<T>
    {
        public InfoAndValue(T info, object value)
        {
            Info = info;
            Value = value;
        }

        public T Info { get; private set; }
        public object Value { get; private set; }
    }
}