using System.Collections.Generic;

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
            return "";
        }

        private static ParserHelpInfo GetParserHelp(ParserRunner parser)
        {
            return new ParserHelpInfo
            {
                Type = parser.Type,
                Verbs = parser.GetVerbs().Select(verb => new VerbHelpInfo
                {
                    Names = verb.Names.Union(new[] { verb.MethodInfo.Name }).ToList(),
                    Description = verb.Description,
                    IsDefault = verb.IsDefault,
                    Validations = verb.MethodInfo.GetInterfaceAttributes<ICollectionValidation>().Select(v => v.Description).ToList(),
                    Parameters = ParserRunner.GetParameters(verb.MethodInfo).
                        Select(p => new ParameterHelpInfo
                        {
                            Required = p.Required,
                            Names = p.Names,
                            Type = p.ParameterInfo.ParameterType,
                            Default = p.Default,
                            Description = p.Description,
                            Validations = p.ParameterInfo.GetAttributes<ValidationAttribute>().Select(v => v.Description).ToList(),
                        }).ToList(),
                }).ToList(),
                Globals = parser.GetDefinedGlobals().
                    Select(g =>
                    {
                        var att = g.GetAttribute<GlobalAttribute>();
                        var parameter = ParserRunner.GetParameters(g).FirstOrDefault();

                        return new ParameterHelpInfo
                        {
                            IsGlobal = true,
                            Names = att.Aliases.CommaSplit().Union(new[] { att.Name ?? g.Name }).ToList(),
                            Type = parameter != null ? parameter.ParameterInfo.ParameterType : typeof(bool),
                            Description = att.Description,
                            Validations = g.GetInterfaceAttributes<ICollectionValidation>().Select(v => v.Description).ToList(),
                        };
                    })
                    .Union(parser.Register.RegisteredGlobalHandlers.Values.Select(handler => new ParameterHelpInfo
                    {
                        Names = handler.Names.ToList(),
                        Type = handler.Type,
                        Description = handler.Desription,
                    })).ToList(),
            };
        }
    }
}