using System;
using System.Collections.Generic;

namespace CLAP
{
    internal class HelpInfo
    {
        internal List<ParserHelpInfo> Parsers { get; set; }
    }

    internal class ParserHelpInfo
    {
        internal Type Type { get; set; }
        internal List<VerbHelpInfo> Verbs { get; set; }
        internal List<GlobalParameterHelpInfo> Globals { get; set; }
    }

    internal class VerbHelpInfo
    {
        internal bool IsDefault { get; set; }
        internal string Description { get; set; }
        internal List<string> Names { get; set; }
        internal List<ParameterHelpInfo> Parameters { get; set; }
        internal List<string> Validations { get; set; }
    }

    internal class ParameterHelpInfo
    {
        internal bool Required { get; set; }
        internal List<string> Names { get; set; }
        internal List<string> Validations { get; set; }
        internal Type Type { get; set; }
        internal object Default { get; set; }
        internal string Description { get; set; }
        internal string Separator { get; set; }
    }

    internal class GlobalParameterHelpInfo
    {
        internal List<string> Names { get; set; }
        internal List<string> Validations { get; set; }
        internal Type Type { get; set; }
        internal string Description { get; set; }
        internal string Separator { get; set; }
    }
}