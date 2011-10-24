using CLAP.Validation;

namespace CLAP
{
    internal static class TypeValidator
    {
        public static void Validate(object obj)
        {
            if (obj == null)
            {
                return;
            }

            if (obj.GetType().HasAttribute<ValidateAttribute>())
            {
                var validators = obj.GetType().GetAttributes<ValidateAttribute>();

                foreach (var v in validators)
                {
                    //v.GetValidator().Validate(
                }
            }
        }
    }
}