
namespace CLAP.Interception
{
    public interface IVerbInterceptor : IPreVerbInterceptor, IPostVerbInterceptor
    {
    }

    public interface IPreVerbInterceptor
    {
        void Intercept(PreVerbExecutionContext context);
    }

    public interface IPostVerbInterceptor
    {
        void Intercept(PostVerbExecutionContext context);
    }
}