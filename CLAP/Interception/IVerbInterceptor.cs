
namespace CLAP.Interception
{
    /// <summary>
    /// Gives interception options to verb execution
    /// </summary>
    public interface IVerbInterceptor : IPreVerbInterceptor, IPostVerbInterceptor
    {
    }

    /// <summary>
    /// Gives interception options to verb execution BEFORE the verb is executed
    /// </summary>
    public interface IPreVerbInterceptor
    {
        void BeforeVerbExecution(PreVerbExecutionContext context);
    }

    /// <summary>
    /// Gives interception options to verb execution AFTER the verb is executed, even if the execution fails
    /// </summary>
    public interface IPostVerbInterceptor
    {
        void AfterVerbExecution(PostVerbExecutionContext context);
    }
}