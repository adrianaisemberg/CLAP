using System;

namespace CLAP
{
    public sealed class ExceptionContext
    {
        public Exception Exception { get; private set; }

        public bool ReThrow { get; set; }

        internal ExceptionContext(Exception ex)
        {
            Exception = ex;
        }
    }
}