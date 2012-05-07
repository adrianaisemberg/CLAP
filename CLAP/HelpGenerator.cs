using System;
using System.Collections.Generic;
using System.Text;

#if !FW2
using System.Linq;
#endif

namespace CLAP
{
    internal static class HelpGenerator
    {
        internal static string GetHelp(MultiParser parser)
        {
            var parsers = parser.Types.Select(t => new ParserRunner(t, parser.Register));

            return GetHelp(parsers);
        }

        internal static string GetHelp(ParserRunner parser)
        {
            return GetHelp(new[] { parser });
        }

        internal static string GetHelp(IEnumerable<ParserRunner> parsers)
        {
            var help = new HelpInfo();

            help.Parsers = parsers.Select(p => GetParserHelp(p)).ToList();

            var helpString = GetHelpString(help);

            return helpString;
        }

        private static string GetHelpString(HelpInfo helpInfo)
        {
            const string verbsLead = "   ";
            const string parametersLead = "        ";
            const string validationsLead = "          ";

            var sb = new StringBuilder();

            var count = helpInfo.Parsers.Count;
            var multi = count > 1;

            for (int i = 0; i < count; i++)
            {
                var parser = helpInfo.Parsers[i];

                foreach (var verb in parser.Verbs.OrderByDescending(v => v.IsDefault))
                {
                    sb.AppendLine();
                    sb.Append(verbsLead);

                    if (multi)
                    {
                        sb.AppendFormat("{0}.", parser.Type.Name.ToLowerInvariant());
                    }

                    sb.Append(verb.Names.StringJoin("|").ToLowerInvariant());

                    if (verb.IsDefault)
                    {
                        sb.Append(" (Default)");
                    }

                    if (!string.IsNullOrEmpty(verb.Description))
                    {
                        sb.AppendFormat(": {0}", verb.Description);
                    }

                    sb.AppendLine();

                    if (verb.Parameters.Any())
                    {
                        var longestParameter = verb.Parameters.Max(p => p.Names.StringJoin(" /").Length);
                        var longestType = verb.Parameters.Max(p => p.Type.GetGenericTypeName().Length);

                        foreach (var p in verb.Parameters.OrderBy(p => p.Names.First()))
                        {
                            sb.Append(parametersLead);
                            sb.AppendFormat("/{0} : ",
                                p.Names.StringJoin(" /").ToLowerInvariant().PadRight(longestParameter, ' '));

                            if (!string.IsNullOrEmpty(p.Description))
                            {
                                sb.AppendFormat("{0} ", p.Description);
                            }

                            var typeName = GetTypeName(p.Type);

                            if (!string.IsNullOrEmpty(typeName))
                            {
                                sb.AppendFormat("({0}) ", typeName);
                            }

                            if (p.Required)
                            {
                                sb.Append("(Required) ");
                            }

                            if (p.Separator != null && p.Separator != SeparatorAttribute.DefaultSeparator)
                            {
                                sb.AppendFormat("(Separator = {0}) ", p.Separator);
                            }

                            if (p.Default != null)
                            {
                                sb.AppendFormat("(Default = {0}) ", p.Default);
                            }

                            if (p.Validations.Any())
                            {
                                sb.AppendFormat("({0}) ", p.Validations.StringJoin(", "));
                            }

                            sb.AppendLine();
                        } // foreach (var p in verb.Parameters
                    }

                    if (verb.Validations.Any())
                    {
                        sb.AppendLine();
                        sb.Append(parametersLead);
                        sb.AppendLine("Validation:");

                        foreach (var v in verb.Validations)
                        {
                            sb.Append(validationsLead);
                            sb.AppendLine(v);
                        }
                    }

                } // foreach (var verb in parser.Verbs

                if (parser.Globals.Any())
                {
                    sb.AppendLine();
                    sb.Append(verbsLead);
                    sb.AppendLine("Global Parameters:");

                    var longestGlobal = parser.Globals.Max(p => p.Names.StringJoin("|").Length);

                    foreach (var g in parser.Globals.OrderBy(g => g.Names.First()))
                    {
                        sb.Append(parametersLead);
                        sb.AppendFormat("/{0} : ",
                            g.Names.StringJoin("|").ToLowerInvariant().PadRight(longestGlobal, ' '));

                        if (!string.IsNullOrEmpty(g.Description))
                        {
                            sb.AppendFormat("{0} ", g.Description);
                        }

                        var typeName = GetTypeName(g.Type);

                        if (!string.IsNullOrEmpty(typeName))
                        {
                            sb.AppendFormat("({0}) ", typeName);
                        }

                        if (g.Separator != null && g.Separator != SeparatorAttribute.DefaultSeparator)
                        {
                            sb.AppendFormat("(Separator = {0}) ", g.Separator);
                        }

                        if (g.Validations != null && g.Validations.Any())
                        {
                            sb.AppendFormat("({0}) ", g.Validations.StringJoin(", "));
                        }

                        sb.AppendLine();
                    } // foreach (var g in parser.Globals
                }


                if (multi && i < count - 1)
                {
                    sb.AppendLine();
                    sb.Append(verbsLead);
                    sb.AppendLine(string.Empty.PadRight(80, '-'));
                }
            }

            return sb.ToString();
        }

        private static string GetTypeName(Type type)
        {
            if (type.IsEnum)
            {
                return string.Format("{0} ({1})", type.Name, string.Join("/", Enum.GetNames(type)));
            }

            if (type == typeof(bool))
            {
                return string.Empty;
            }

            return type.GetGenericTypeName();
        }

        private static ParserHelpInfo GetParserHelp(ParserRunner parser)
        {
            var h = new ParserHelpInfo
            {
                Type = parser.Type,
                Verbs = parser.GetVerbs().Select(verb => new VerbHelpInfo
                {
                    Names = verb.Names.OrderByDescending(n => n.Length).ToList(),
                    Description = verb.Description,
                    IsDefault = verb.IsDefault,
                    Validations = verb.MethodInfo.GetInterfaceAttributes<ICollectionValidation>().Select(v => v.Description).ToList(),
                    Parameters = ParserRunner.GetParameters(verb.MethodInfo).
                        Select(p => new ParameterHelpInfo
                        {
                            Required = p.Required,
                            Names = p.Names.OrderBy(n => n.Length).ToList(),
                            Type = p.ParameterInfo.ParameterType,
                            Default = p.DefaultProvider != null ? p.DefaultProvider.Description : p.Default,
                            Description = p.Description,
                            Validations = p.ParameterInfo.GetAttributes<ValidationAttribute>().Select(v => v.Description).ToList(),
                            Separator = p.ParameterInfo.ParameterType.IsArray ?
                                p.Separator ?? SeparatorAttribute.DefaultSeparator : null,
                        }).ToList(),
                }).ToList(),
                Globals = parser.GetDefinedGlobals().Select(g =>
                {
                    var att = g.GetAttribute<GlobalAttribute>();
                    var parameter = ParserRunner.GetParameters(g).FirstOrDefault();

                    return new GlobalParameterHelpInfo
                    {
                        Names = att.Aliases.CommaSplit().Union(new[] { att.Name ?? g.Name }).OrderBy(n => n.Length).ToList(),
                        Type = parameter != null ? parameter.ParameterInfo.ParameterType : typeof(bool),
                        Description = att.Description,
                        Validations = g.GetInterfaceAttributes<ICollectionValidation>().Select(v => v.Description).ToList(),
                        Separator = parameter != null ?
                            parameter.ParameterInfo.ParameterType.IsArray ?
                                parameter.Separator ?? SeparatorAttribute.DefaultSeparator :
                                    null : null,
                    };
                }).Union(parser.Register.RegisteredGlobalHandlers.Values.Select(handler => new GlobalParameterHelpInfo
                {
                    Names = handler.Names.OrderBy(n => n.Length).ToList(),
                    Type = handler.Type,
                    Description = handler.Description,
                    Validations = new List<string>(),
                })).ToList(),
            };

            if (parser.Register.RegisteredHelpHandlers.Any())
            {
                h.Globals.Add(new GlobalParameterHelpInfo
                {
                    Names = parser.Register.RegisteredHelpHandlers.Keys.ToList(),
                    Type = typeof(bool),
                    Description = "Help",
                });
            };

            return h;
        }
    }
}