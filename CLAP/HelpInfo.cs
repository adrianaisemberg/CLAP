using System;
using System.Collections.Generic;

namespace CLAP
{
    public class HelpInfo
    {
        public List<ParserHelpInfo> Parsers { get; set; }
    }

    public class ParserHelpInfo
    {
        public Type Type { get; set; }
        public List<VerbHelpInfo> Verbs { get; set; }
        public List<GlobalParameterHelpInfo> Globals { get; set; }
    }

    public class VerbHelpInfo
    {
        public bool IsDefault { get; set; }
        public string Description { get; set; }
        public List<string> Names { get; set; }
        public List<ParameterHelpInfo> Parameters { get; set; }
        public List<string> Validations { get; set; }
    }

    public class ParameterHelpInfo
    {
        public bool Required { get; set; }
        public List<string> Names { get; set; }
        public List<string> Validations { get; set; }
        public Type Type { get; set; }
        public object Default { get; set; }
        public string Description { get; set; }
        public string Separator { get; set; }
    }

    public class GlobalParameterHelpInfo
    {
        public List<string> Names { get; set; }
        public List<string> Validations { get; set; }
        public Type Type { get; set; }
        public string Description { get; set; }
        public string Separator { get; set; }
    }
}