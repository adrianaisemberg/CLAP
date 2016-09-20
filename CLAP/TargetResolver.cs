using System;
using System.Collections.Generic;

#if !NET20
using System.Linq;
#endif
namespace CLAP
{
    public class TargetResolver
    {
        private readonly Dictionary<Type, Func<object>> targetHash = new Dictionary<Type, Func<object>>();

        public TargetResolver()
        {
        }

        public TargetResolver(params object[] targets)
        {
            foreach (var target in targets)
            {
                var innerTarget = target;
                RegisterTargetType(innerTarget.GetType(), () => innerTarget);
            }
        }

        public void RegisterTargetType<T>(Func<T> resolver)
            where T: class
        {
            RegisterTargetType(typeof (T), () => resolver());
        }

        private void RegisterTargetType(Type targetType, Func<object> resolver)
        {
            if (targetHash.ContainsKey(targetType))
                throw new ArgumentException("The provided type is already registered.");

            targetHash.Add(targetType, resolver);
        }

        internal Type[] RegisteredTypes { get { return targetHash.Keys.ToArray(); } }

        internal object Resolve(Type targetType)
        {
            
            if (!targetHash.ContainsKey(targetType))
            {
                throw new ArgumentException("The requested type is not registered.");
            }

            return targetHash[targetType]();
        }
    }
}