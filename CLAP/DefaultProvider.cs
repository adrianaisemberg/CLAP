
namespace CLAP
{
    public abstract class DefaultProvider
    {
        public abstract object GetDefault(VerbExecutionContext context);

        public virtual string Description
        {
            get
            {
                return GetType().Name;
            }
        }
    }
}