using System.Data;

#if !NET20
using System.Linq;
#endif

namespace CLAP
{
#if NET20 || NET452
    /// <summary>
    /// Validates a collection of named parameters or properties against a boolean expression
    /// </summary>
    internal class ParametersExpressionValidator : ICollectionValidator
    {
        /// <summary>
        /// The expression
        /// </summary>
        public string Expression { get; private set; }

        /// <summary>
        /// Whether the expression should be treated as case-sensitive
        /// </summary>
        public bool CaseSensitive { get; private set; }

        internal ParametersExpressionValidator(string expression, bool caseSensitive)
        {
            Expression = expression;
            CaseSensitive = caseSensitive;
        }

        public void Validate(ValueInfo[] parameters)
        {
            var table = new DataTable();

            table.CaseSensitive = CaseSensitive;

            // create a column for each parameter giving its name and type
            //
            table.Columns.AddRange(
                parameters.Select(
                    p => new DataColumn(
                            p.Name,
                            p.Type)).ToArray());

            // create one row with all the values
            //
            table.Rows.Add(parameters.Select(p => p.Value).ToArray());

            // run the expression
            //
            var selected = table.Select(Expression);

            if (!selected.Any())
            {
                throw new ValidationException("Expression failed validation: '{0}' for arguments: [{1}]".FormatWith(
                    Expression,
                    parameters.Select(
                        p => "{0}={1}".FormatWith(
                            p.Name,
                            p.Value.ToSafeString("<NULL>"))).StringJoin(", ")));
            }
        }
    }
#endif
}