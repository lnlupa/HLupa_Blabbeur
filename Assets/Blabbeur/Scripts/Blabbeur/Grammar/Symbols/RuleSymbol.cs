namespace Blabbeur
{
    namespace Grammar
    {
        /// <summary>
        /// A Symbol which refers to another production rule in the grammar. It will expand recursively, calling the production rule it refers to.
        /// </summary>
        public class RuleSymbol : ISymbol
        {
            /// <summary>
            /// The symbol type.
            /// </summary>
            public SymbolType Type => SymbolType.RULE;

            /// <summary>
            /// The raw text of the symbol.
            /// </summary>
            private string rawText;

            public string RawText => rawText;

            /// <summary>
            /// The RuleSymbol constructor
            /// </summary>
            /// <param name="_text">The raw text, in this case, the production rule tag.</param>
            public RuleSymbol(string _text) => rawText = _text;

            /// <summary>
            /// Expand the symbol by recursively getting the expansion of the production rule it refers to.
            /// </summary>
            /// <param name="depth">The current depth of the symbol.</param>
            /// <returns>The expansion of the symbol</returns>
            public string Expand(int depth)
            {
                if (depth < TextGen.MAXDEPTH)
                    return TextGen.ActiveGrammar.Expand(rawText, depth);
                else
                    return TextGen.MAXDEPTHERROR;
            }
        }
    }
}