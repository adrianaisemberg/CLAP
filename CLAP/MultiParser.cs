using System;
using System.Diagnostics;
using System.Globalization;

#if NETSTANDARD1_6
using System.Reflection;
#endif

#if !NET20
using System.Linq;
#endif

namespace CLAP
{
    /// <summary>
    /// A parser of one or more classes
    /// </summary>
    public abstract class MultiParser
    {
        #region Fields

        private static readonly string[] s_delimiters = new[] { ".", "/" };
        private readonly Type[] m_types;
        public HelpGeneratorBase HelpGenerator = new DefaultHelpGenerator();

        internal const int ErrorCode = 1;
        internal const int SuccessCode = 0;

        #endregion Fields

        #region Properties

        internal Type[] Types
        {
            get { return m_types; }
        }

        /// <summary>
        /// Parser registration
        /// </summary>
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
            
#if NETSTANDARD1_6
            m_types = GetType().GetTypeInfo().GetGenericArguments();
#else
            m_types = GetType().GetGenericArguments();
#endif
            Init();
        }

        #endregion Constructors

        #region Private Methods

        private void Init()
        {
            Debug.Assert(m_types.Any());

            Register = new ParserRegistration(m_types, GetHelpString);

            foreach (var type in m_types)
            {
                ParserRunner.Validate(type, Register);
            }
        }

        private void HandleEmptyArguments(TargetResolver targetResolver)
        {
            if (Register.RegisteredEmptyHandler != null)
            {
                Register.RegisteredEmptyHandler();
            }
            else if (m_types.Length == 1)
            {
                var parser = new ParserRunner(m_types.First(), Register, HelpGenerator);

                var target = targetResolver == null ? null : targetResolver.Resolve(m_types[0]);

                parser.HandleEmptyArguments(target);
            }
        }

        private ParserRunner GetMultiTypesParser(string[] args, ParserRegistration registration)
        {
            Debug.Assert(args.Any());

            var verb = args[0];

            // if the first arg is not a verb - throw
            //
            if (verb.StartsWith(ParserRunner.ArgumentPrefixes))
            {
                throw new MissingVerbException();
            }

            if (!verb.Contains(s_delimiters))
            {
                throw new MultiParserMissingClassNameException();
            }

            var parts = verb.Split(s_delimiters, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
            {
                throw new InvalidVerbException();
            }

            var typeNameOrAlias = parts[0];

            args[0] = args[0].Substring(typeNameOrAlias.Length + 1);

            var matchingType = registration.GetTargetType(typeNameOrAlias);

            if (matchingType == null)
            {
                throw new UnknownParserTypeException(typeNameOrAlias);
            }

return new ParserRunner(matchingType, registration, HelpGenerator);        }

        

        private ParserRunner GetSingleTypeParser(string[] args, ParserRegistration registration)
        {
            Debug.Assert(m_types.Length == 1);

            var type = m_types.First();

            var verb = args[0];

            var parser = new ParserRunner(type, registration, HelpGenerator);

            // if there is no verb - leave all the args as is
            //
            if (verb.StartsWith(ParserRunner.ArgumentPrefixes))
            {
                return parser;
            }

            // if the verb contains a delimiter - remove the type name from the arg
            //
            if (verb.Contains(s_delimiters))
            {
                var parts = verb.Split(s_delimiters, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length != 2)
                {
                    throw new InvalidVerbException();
                }

                Debug.Assert(parts.Length == 2);

                var typeName = parts[0];

                if (!type.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase))
                {
                    throw new UnknownParserTypeException(typeName);
                }

                args[0] = args[0].Substring(typeName.Length + 1);
            }

            return parser;
        }

        #endregion Private Methods

        #region Public Methods

        /// <summary>
        /// Run a parser of static verbs
        /// </summary>
        /// <param name="args">The user arguments</param>
        public int RunStatic(string[] args)
        {
            return RunTargets(args, null as TargetResolver);
        }

        /// <summary>
        /// Run a parser of instance verbs against instances of the verb classes
        /// </summary>
        /// <param name="args">The user arguments</param>
        /// <param name="targets">The instances of the verb classes</param>
        public int RunTargets(string[] args, params object[] targets)
        {
            var targetResolver = new TargetResolver(targets);
            return RunTargets(args, targetResolver);
        }

        public int RunTargets(string[] args, TargetResolver targetResolver)
        {
            ParserRunner parser;

            try
            {
                if (args.None() || args.All(a => string.IsNullOrEmpty(a)))
                {
                    HandleEmptyArguments(targetResolver);
                    return SuccessCode;
                }

                if (m_types.Length == 1)
                {
                    parser = GetSingleTypeParser(args, Register);
                }
                else
                {
                    Debug.Assert(m_types.Length > 1);
                    parser = GetMultiTypesParser(args, Register);
                }

                Debug.Assert(parser != null);
            }
            catch (Exception ex)
            {
                // handle error using the first available error handler
                //
                // (if returns true - should rethrow)
                //
                if (TryHandlePrematureError(ex, targetResolver))
                {
                    throw;
                }
                else
                {
                    return ErrorCode;
                }
            }

            var target = (targetResolver == null || targetResolver.RegisteredTypes.None()) ? null : targetResolver.Resolve(parser.Type);

            return parser.Run(args, target);
        }

        private bool TryHandlePrematureError(Exception ex, TargetResolver targetResolver)
        {
            var context = new ExceptionContext(ex);

            if (Register.RegisteredErrorHandler != null)
            {
                Register.RegisteredErrorHandler(context);

                return context.ReThrow;
            }
            else
            {
                for (int i = 0; i < m_types.Length; i++)
                {
                    var type = m_types[i];

                    var errorHandler = ParserRunner.GetDefinedErrorHandlers(type).FirstOrDefault();

                    if (errorHandler != null)
                    {
                        var target = targetResolver == null ? null : targetResolver.Resolve(type);

                        errorHandler.Invoke(target, new[] { context });

                        return context.ReThrow;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Gets a help string that describes all the parser information for the user
        /// </summary>
        public string GetHelpString()
        {
            return HelpGenerator.GetHelp(Types.Select(t => new ParserRunner(t, Register, HelpGenerator)));
        }

        #endregion Public Methods
    }
}