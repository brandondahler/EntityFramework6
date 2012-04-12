namespace System.Data.Entity.Core.Common.EntitySql.AST
{
    using System.Data.Entity.Resources;
    using System.Diagnostics;

    /// <summary>
    /// Represents an identifier ast node.
    /// </summary>
    internal sealed class Identifier : Node
    {
        private readonly string _name;
        private readonly bool _isEscaped;

        /// <summary>
        /// Initializes identifier.
        /// </summary>
        internal Identifier(string name, bool isEscaped, string query, int inputPos)
            : base(query, inputPos)
        {
            // name may be empty in the case of "byte[]". 
            // "byte" and "[]" come in as two identifiers where second one is escaped and empty.

            Debug.Assert(isEscaped || name[0] != '[', "isEscaped || name[0] != '['");

            if (!isEscaped)
            {
                var isIdentifierASCII = true;
                if (!CqlLexer.IsLetterOrDigitOrUnderscore(name, out isIdentifierASCII))
                {
                    if (isIdentifierASCII)
                    {
                        throw EntityUtil.EntitySqlError(ErrCtx, Strings.InvalidSimpleIdentifier(name));
                    }
                    else
                    {
                        throw EntityUtil.EntitySqlError(ErrCtx, Strings.InvalidSimpleIdentifierNonASCII(name));
                    }
                }
            }

            _name = name;
            _isEscaped = isEscaped;
        }

        /// <summary>
        /// Returns identifier name (without escaping chars).
        /// </summary>
        internal string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// True if an identifier is escaped.
        /// </summary>
        internal bool IsEscaped
        {
            get { return _isEscaped; }
        }
    }
}
