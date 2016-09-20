using System.Collections.Generic;

#if !NET20
using System.Linq;
#endif

namespace CLAP
{
    public abstract class HelpGeneratorBase
    {
        internal string GetHelp(ParserRunner parser)
        {
            return GetHelp(new[] { parser });
        }

        internal string GetHelp(IEnumerable<ParserRunner> parsers)
        {
            var help = new HelpInfo {Parsers = parsers.Select(GetParserHelp).ToList()};

            var helpString = GetHelpString(help);

            return helpString;
        }

        protected abstract string GetHelpString(HelpInfo helpInfo);

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
            }

            return h;
        }
    }
}