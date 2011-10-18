using System;
using System.Data;
using System.Linq;
using CLAP;

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
    public sealed class ValidateExpressionAttribute : ParametersValidationAttribute
    {
        /// <summary>
        /// The expression to validate
        /// </summary>
        public string Expression { get; private set; }

        /// <summary>
        /// Whether to use case-sensitive comparison when validating the expression
        /// </summary>
        public bool CaseSensitive { get; set; }

        /// <summary>
        /// Validates all the parameters against an expression
        /// </summary>
        /// <example>param1 > param2</example>
        /// <remarks>
        /// For full documentation, see MSDN:
        /// http://msdn.microsoft.com/en-us/library/system.data.datacolumn.expression.aspx
        /// </remarks>
        /// <param name="expression">The expression to validate</param>
        public ValidateExpressionAttribute(string expression)
        {
            Expression = expression;
        }

        public override IParametersValidator GetValidator()
        {
            return new ParametersExpressionValidator(Expression, CaseSensitive);
        }

        public override string Description
        {
            get
            {
                return "Matches expression: '{0}'".FormatWith(Expression);
            }
        }

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

                table.Columns.AddRange(
                    parameters.Select(
                        p => new DataColumn(
                                p.Parameter.Name,
                                p.Parameter.ParameterType)).ToArray());

                table.Rows.Add(parameters.Select(p => p.Value).ToArray());

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
    }
}