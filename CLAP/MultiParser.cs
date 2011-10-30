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

        protected MultiParser(params Type[] types)
        {
            m_types = types;

            Init();
        }

        protected MultiParser()
        {
            m_types = GetType().GetGenericArguments();

            Init();
        }

        #endregion Constructors

        #region Methods

        private void Init()
        {
            Debug.Assert(m_types.Any());

            foreach (var type in m_types)
            {
                ParserRunner.Validate(type);
            }

            Register = new ParserRegistration(GetHelpString, ValuesFactory.GetValueForParameter);
        }

        public void StaticRun(string[] args)
        {
            RunTargets(args, null);
        }

        internal void RunTargets(string[] args, params object[] targets)
        {
            // no args
            //
            if (args.None() || args.All(a => string.IsNullOrEmpty(a)))
            {
                HandleEmptyArguments(targets);

                return;
            }

            ParserRunner parser;

            if (m_types.Length == 1)
            {
                parser = GetSingleTypeParser(args, targets, Register);
            }
            else
            {
                Debug.Assert(m_types.Length > 1);

                parser = GetMultiTypesParser(args, targets, Register);
            }

            Debug.Assert(parser != null);

            var index = m_types.IndexOf(parser.Type);

            Debug.Assert(index >= 0);

            var target = targets.None() ? null : targets[index];

            parser.Run(args, target);
        }

        private void HandleEmptyArguments(object[] targets)
        {
            if (Register.RegisteredEmptyHandler != null)
            {
                Register.RegisteredEmptyHandler();
            }
            else if (m_types.Length == 1)
            {
                var parser = new ParserRunner(m_types.First(), Register);

                var target = targets == null ? null : targets[0];

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
            Debug.Assert(args.Any());

            var verb = args[0];

            // if the first arg is not a verb - throw
            //
            if (verb.StartsWith(ParserRunner.ArgumentPrefixes))
            {
                throw new MissingVerbException();
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
            if (verb.StartsWith(ParserRunner.ArgumentPrefixes))
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