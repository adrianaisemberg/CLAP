using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CLAP
{
    /// <summary>
    /// Information about a verb being invoked.
    /// </summary>
    public interface IVerbInvocation
    {
        /// <summary>
        /// Name of the verb to be invoked
        /// </summary>
        string Verb { get; }

        /// <summary>
        /// Method descriptor for verb
        /// </summary>
        Method VerbMethod { get; }

        /// <summary>
        /// List of parameter values passed to the method
        /// </summary>
        ReadOnlyCollection<object> VerbParameters { get; }

        /// <summary>
        /// List of raw input parameters
        /// </summary>
        ReadOnlyCollection<string> InputArgs { get; }

        /// <summary>
        /// Dictionary of input parameters by name
        /// </summary>
        IDictionary<string, string> InputDictionary { get; }

        /// <summary>
        /// Object associated with the method.  This will be
        /// null if the method is static.
        /// </summary>
        object TargetObject { get; }

        /// <summary>
        /// Invokes the verb.
        /// </summary>
        void Proceed();
    }
}