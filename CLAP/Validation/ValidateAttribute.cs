using System;
using System.Data;
using System.Linq;

namespace CLAP.Validation
{
    /// <summary>
    /// Validates all the parameters against an expression
    /// </summary>
    /// <example>param1 > param2</example>
    /// <remarks>
    /// For full documentation, see MSDN:
    /// http://msdn.microsoft.com/en-us/library/system.data.datacolumn.expression.aspx
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public sealed class ValidateAttribute : ParametersValidationAttribute
    {
        #region Properties

        /// <summary>
        /// The expression to validate
        /// </summary>
        public string Expression { get; private set; }

        /// <summary>
        /// Whether to use case-sensitive comparison when validating the expression
        /// </summary>
        public bool CaseSensitive { get; set; }

        public override string Description
        {
            get
            {
                return "Matches expression: '{0}'".FormatWith(Expression);
            }
        }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Validates all the parameters against an expression
        /// </summary>
        /// <example>param1 > param2</example>
        /// <remarks>
        /// For full documentation, see MSDN:
        /// http://msdn.microsoft.com/en-us/library/system.data.datacolumn.expression.aspx
        /// </remarks>
        /// <param name="expression">The expression to validate</param>
        public ValidateAttribute(string expression)
        {
            Expression = expression;
        }

        #endregion Constructors

        #region Methods

        public override IParametersValidator GetValidator()
        {
            return new ParametersExpressionValidator(Expression, CaseSensitive);
        }

        #endregion Methods

        #region Types

        private class ParametersExpressionValidator : IParametersValidator
        {
            public string Expression { get; private set; }
            public bool CaseSensitive { get; private set; }

            internal ParametersExpressionValidator(string expression, bool caseSensitive)
            {
                Expression = expression;
                CaseSensitive = caseSensitive;
            }

            public void Validate(ParameterInfoAndValue[] parameters)
            {
                var table = new DataTable();

                table.CaseSensitive = CaseSensitive;

                // create a column for each parameter giving its name and type
                //
                table.Columns.AddRange(
                    parameters.Select(
                        p => new DataColumn(
                                p.Parameter.Name,
                                p.Parameter.ParameterType)).ToArray());

                // create one row with all the values
                //
                table.Rows.Add(parameters.Select(p => p.Value).ToArray());

                // run the expression
                //
                var selected = table.Select(Expression);

                if (!selected.Any())
                {
                    throw new ValidationException(string.Format("Expression failed validation: '{0}' for arguments: [{1}]",
                        Expression,
                        parameters.Select(
                            p => "{0}={1}".FormatWith(
                                p.Parameter.Name,
                                p.Value.ToSafeString("<NULL>"))).StringJoin(", ")));
                }
            }
        }

        #endregion Types
    }
}