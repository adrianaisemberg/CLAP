using System;
using System.Reflection;
using System.Text;

namespace CLAP
{
    internal static class HelpGenerator
    {
        internal static string GetHelp(MultiParser parser)
        {
            var sb = new StringBuilder();

            foreach (var type in parser.Types)
            {
                var pr = new ParserRunner(type, parser.Register);

                sb.AppendLine(type.Name);
                sb.AppendLine("".PadLeft(30, '-'));

                sb.AppendLine(GetHelp(pr));

                sb.AppendLine();
            }

            return sb.ToString();
        }

        internal static string GetHelp(ParserRunner parser)
        {
            var verbs = parser.GetVerbs().ToList();

            if (verbs.None())
            {
                return string.Empty;
            }

            var sb = new StringBuilder();

            for (int i = 0; i < verbs.Count; i++)
            {
                var verb = verbs[i];

                sb.AppendLine();

                // verb name (title)
                //
                var name = verb.IsDefault ? verb.MethodInfo.Name + " [DEFAULT]" : verb.MethodInfo.Name;

                sb.AppendLine(name);
                sb.AppendLine(string.Empty.PadLeft(name.Length, '~'));

                // description
                //
                if (!string.IsNullOrEmpty(verb.Description))
                {
                    sb.AppendLine(verb.Description);

                    sb.AppendLine();
                }

                // all names
                //
                sb.Append(
                    verb.Names.
                    OrderBy(n => n.Length).
                    Select(n => n.ToUpperInvariant()).
                    StringJoin("|")).Append(" ");

                //  parameters
                //
                var parameters = ParserRunner.GetParameters(verb.MethodInfo);

                foreach (var p in parameters)
                {
                    if (!p.Required)
                    {
                        sb.Append("[");
                    }

                    sb.AppendFormat("/{0}", GetParameterNameAndType(p));

                    if (!p.Required)
                    {
                        sb.Append("]");
                    }

                    sb.Append(" ");
                }

                sb.AppendLine();

                // parameters options
                //
                if (parameters.Any())
                {
                    sb.AppendLine();
                }

                foreach (var p in parameters)
                {
                    sb.Append("    /");
                    sb.Append(GetParameterNameAndType(p));
                    sb.AppendLine(GetParameterOption(p));
                }

                // verb validation
                //
                var validators = verb.MethodInfo.GetInterfaceAttributes<ICollectionValidation>();

                if (validators.Any())
                {
                    sb.AppendLine();
                    sb.AppendLine("Validation:");
                    sb.AppendLine("-----------");

                    foreach (var validator in validators)
                    {
                        sb.AppendLine("    - {0}".FormatWith(validator.Description));
                    }
                }

                sb.AppendLine();

                // don't add a line after the last verb
                //
                if (i != verbs.Count - 1)
                {
                    sb.AppendLine(string.Empty.PadLeft(80, '='));
                }
            }

            // globals
            //
            var definedGlobals = parser.GetDefinedGlobals();

            if (parser.Register.RegisteredGlobalHandlers.Any() || definedGlobals.Any())
            {
                sb.AppendLine();
                sb.AppendLine("Global Parameters:");
                sb.AppendLine("------------------");
                sb.AppendLine();

                foreach (var handler in parser.Register.RegisteredGlobalHandlers.Values)
                {
                    sb.AppendLine("    /{0}=<{1}>".FormatWith(
                        handler.Names.
                            OrderBy(n => n.Length).
                            Select(n => n.ToUpperInvariant()).
                            StringJoin("|"),
                        handler.Type.GetGenericTypeName()));

                    if (!string.IsNullOrEmpty(handler.Desription))
                    {
                        sb.AppendLine("        {0}".FormatWith(handler.Desription));
                        sb.AppendLine();
                    }
                }

                foreach (var handler in definedGlobals)
                {
                    sb.AppendLine("    /{0}".FormatWith(GetDefinedGlobalHelpString(handler)));
                }
            }

            return sb.ToString();
        }

        private static string GetDefinedGlobalHelpString(MethodInfo method)
        {
            var sb = new StringBuilder();

            var globalAtt = method.GetAttribute<GlobalAttribute>();

            // name
            //
            var name = globalAtt.Name ?? method.Name;

            var names = globalAtt.Aliases.CommaSplit().
                Union(new[] { name }).
                OrderBy(n => n.Length).
                Select(n => n.ToUpperInvariant()).
                StringJoin("|");

            sb.Append(names);

            // type
            //
            var parameters = ParserRunner.GetParameters(method);
            if (parameters.Any())
            {
                // can't be more than one parameter
                //
                var p = parameters.First();

                sb.Append("=<{0}>".FormatWith(p.ParameterInfo.ParameterType.GetGenericTypeName()));

                sb.Append(GetParameterOption(p));
            }
            else // bool switch
            {

            }

            var lineAdded = false;

            // description
            //
            if (!string.IsNullOrEmpty(globalAtt.Description))
            {
                sb.AppendLine();
                sb.AppendLine("        {0}".FormatWith(globalAtt.Description));

                lineAdded = true;
            }

            // validations
            //
            var validators = method.GetInterfaceAttributes<ICollectionValidation>();

            if (validators.Any() && !lineAdded)
            {
                sb.AppendLine();
            }

            foreach (var validator in validators)
            {
                sb.AppendLine("        {0}".FormatWith(validator.Description));
            }

            return sb.ToString();
        }

        private static string GetParameterNameAndType(Parameter p)
        {
            var sb = new StringBuilder();

            sb.Append(p.Names.
                OrderBy(n => n.Length).
                Select(n => n.ToUpperInvariant()).
                StringJoin("|"));

            // not a switch - print the type
            //
            if (p.ParameterInfo.ParameterType != typeof(bool))
            {
                sb.AppendFormat("=<{0}>", p.ParameterInfo.ParameterType.GetGenericTypeName());
            }

            // if it is a required switch
            //
            else if (p.Required)
            {
                sb.Append("[=<true|false>]");
            }

            return sb.ToString();
        }

        private static string GetParameterOption(Parameter p)
        {
            var sb = new StringBuilder();

            // required
            //
            if (p.Required)
            {
                sb.Append(", Required");
            }

            // default value
            //
            if (p.Default != null)
            {
                sb.AppendFormat(", Default = {0}", p.Default);
            }

            // enum values
            //
            if (p.ParameterInfo.ParameterType.IsEnum)
            {
                sb.AppendLine();
                sb.AppendLine("        Options:");
                sb.AppendLine("        --------");

                var values = Enum.GetNames(p.ParameterInfo.ParameterType);

                foreach (var v in values)
                {
                    sb.AppendLine("        {0}".FormatWith(v));
                }
            }

            var lineAdded = false;

            // description
            //
            if (!string.IsNullOrEmpty(p.Description))
            {
                sb.AppendLine();
                sb.AppendLine("        {0}".FormatWith(p.Description));

                lineAdded = true;
            }

            // validations
            //
            var validationAttributes = p.ParameterInfo.GetAttributes<ValidationAttribute>();

            if (validationAttributes.Any() && !lineAdded)
            {
                sb.AppendLine();
            }

            foreach (var validationAttribute in validationAttributes)
            {
                sb.AppendLine("        {0}".FormatWith(validationAttribute.Description));
            }

            return sb.ToString();
        }
    }
}