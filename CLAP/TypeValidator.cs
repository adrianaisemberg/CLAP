using System.Linq;
using System.Reflection;
using CLAP.Validation;

namespace CLAP
{
    internal static class TypeValidator
    {
        public static void Validate(object obj)
        {
            if (obj == null ||
                obj is string)
            {
                return;
            }

            var type = obj.GetType();

            if (type.IsArray ||
                type.IsEnum ||
                type.Assembly.GlobalAssemblyCache)
            {
                return;
            }

            var validators = type.
                GetAttributes<ValidateAttribute>().
                Cast<IValidation>().
                Select(a => a.GetValidator());

            var propsAndValues = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).
                Select(p => new ValueInfo(p.Name, p.PropertyType, p.GetValue(obj, null))).
                ToArray();

            foreach (var validator in validators)
            {
                validator.Validate(propsAndValues);
            }

            // recursion
            //
            foreach (var value in propsAndValues.Select(p => p.Value))
            {
                Validate(value);
            }
        }
    }
}