using System.Diagnostics;
using System.Reflection;

namespace CLAP
{
    public static class MethodInvoker
    {
        public static IMethodInvoker Invoker { get; set; }

        static MethodInvoker()
        {
            Invoker = new DefaultMethodInvoker();
        }

        public static void Invoke(MethodInfo method, object obj, object[] parameters)
        {
            Debug.Assert(method != null);

            Invoker.Invoke(method, obj, parameters);
        }

        private class DefaultMethodInvoker : IMethodInvoker
        {
            public void Invoke(MethodInfo method, object obj, object[] parameters)
            {
                method.Invoke(obj, parameters);
            }
        }
    }

    public interface IMethodInvoker
    {
        void Invoke(MethodInfo method, object obj, object[] parameters);
    }
}