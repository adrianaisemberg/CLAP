using System;

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
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
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

        public override IInfoValidator GetValidator()
        {
            return new ParametersExpressionValidator(Expression, CaseSensitive);
        }

        #endregion Methods
    }
}