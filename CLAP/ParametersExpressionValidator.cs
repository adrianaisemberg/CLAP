﻿using System.Data;
using System.Linq;

namespace CLAP
{
    internal class ParametersExpressionValidator : IInfoValidator
    {
        public string Expression { get; private set; }
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
                throw new ValidationException(string.Format("Expression failed validation: '{0}' for arguments: [{1}]",
                    Expression,
                    parameters.Select(
                        p => "{0}={1}".FormatWith(
                            p.Name,
                            p.Value.ToSafeString("<NULL>"))).StringJoin(", ")));
            }
        }
    }
}
