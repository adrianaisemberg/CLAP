using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLAP
{
    /// <summary>
    /// Defines a term which when at the start of the args list causes all args to be ignored.
    /// </summary>
    /// <remarks>
    /// This is intended to simplify testing and debugging where you can simply prepend this in 
    /// the Visual Studio debugger arguments textbox to mask all args without having to remove them.
    /// </remarks>
    [Serializable]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class HideAllAttribute : Attribute
    {
        public HideAllAttribute(string Term)
        {
            // We may want to insists this is a simple text only string, no spaces, digits etc

            if (Term == null)
                this.Term = "";
            else
                this.Term = Term;
        }

        /// <summary>
        /// If the string defined here is the first argument in an arglist then all args are ignored.
        /// </summary>
        public string Term { get; private set; }
    }
}
