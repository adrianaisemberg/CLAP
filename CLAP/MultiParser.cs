using System;
using System.Diagnostics;
using System.Linq;

namespace CLAP
{
    public abstract class MultiParser
    {
        #region Fields

        private static readonly string[] s_delimiters = new[] { ".", "/" };
        private readonly Type[] m_types;

        #endregion Fields

        #region Properties

        public ParserRegistration Register { get; private set; }

        #endregion Properties

        #region Constructors

        public MultiParser()
        {
            m_types = GetType().GetGenericArguments();

            foreach (var type in m_types)
            {
                ParserRunner.Validate(type);
            }

            Register = new ParserRegistration(GetHelpString, ValuesFactory.GetValueForParameter);
        }

        #endregion Constructors

        #region Methods

        public void Run(string[] args)
        {
            Run(args, null);
        }

        public void Run(string[] args, object obj)
        {
            // no args
            //
            if (args.None() || args.All(a => string.IsNullOrEmpty(a)))
            {
                HandleEmptyArguments(obj);

                return;
            }

            ParserRunner parser;

            if (m_types.Length == 1)
            {
                parser = GetSingleTypeParser(args, obj, Register);
            }
            else
            {
                Debug.Assert(m_types.Length > 1);

                parser = GetMultiTypesParser(args, obj, Register);
            }

            Debug.Assert(parser != null);

            parser.Run(args, obj);
        }

        private void HandleEmptyArguments(object target)
        {
            if (Register.RegisteredEmptyHandler != null)
            {
                Register.RegisteredEmptyHandler();
            }
            else if (m_types.Length == 1)
            {
                var parser = new ParserRunner(m_types.First(), Register);

                parser.HandleEmptyArguments(target);
            }
        }

        public string GetHelpString()
        {
            return "FOOOO!";
        }

        #endregion Methods

        private ParserRunner GetMultiTypesParser(string[] args, object obj, ParserRegistration registration)
        {
            if (args.Length == 0)
            {
#warning TODO:
                throw new Exception("Multi parser needs a verb");
            }

            var verb = args[0];

            if (ParserRunner.Prefixes.Any(p => verb.StartsWith(p)))
            {
#warning TODO:
                throw new Exception("no verb");
            }

            if (!s_delimiters.Any(d => verb.Contains(d)))
            {
#warning TODO:
                throw new Exception("requires dot");
            }

            var parts = verb.Split(s_delimiters, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length > 2)
            {
                throw new Exception("parts length != 2");
            }

            var typeName = parts[0];

            args[0] = args[0].Substring(typeName.Length + 1);

            var type = m_types.FirstOrDefault(t => t.Name.Equals(typeName, StringComparison.InvariantCultureIgnoreCase));

            if (type == null)
            {
                throw new Exception("no type");
            }

            return new ParserRunner(type, registration);
        }

        private ParserRunner GetSingleTypeParser(string[] args, object obj, ParserRegistration registration)
        {
            Debug.Assert(m_types.Length == 1);

            var type = m_types.First();

            var verb = args[0];

            var parser = new ParserRunner(type, registration);

            // if there is no verb - leave all the args as is
            //
            if (ParserRunner.Prefixes.Any(p => verb.StartsWith(p)))
            {
                return parser;
            }

            // if the verb contains a delimiter - remove the type name from the arg
            //
            if (s_delimiters.Any(d => verb.Contains(d)))
            {
                var parts = verb.Split(s_delimiters, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length > 2)
                {
#warning TODO:
                    throw new Exception("parts length != 2");
                }

                Debug.Assert(parts.Length == 2);

                var typeName = parts[0];

                args[0] = args[0].Substring(typeName.Length + 1);
            }

            return parser;
        }
    }
}