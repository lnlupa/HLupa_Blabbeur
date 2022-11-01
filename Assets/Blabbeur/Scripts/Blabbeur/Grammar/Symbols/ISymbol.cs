namespace Blabbeur
{
    namespace Grammar
    {
        /// <summary>
        /// Symbol Interface for a Blabbeur Context-Free Grammar Symbol
        /// </summary>
        public interface ISymbol
        {
            /// <summary>
            /// All symbols must have a symbol type
            /// </summary>
            SymbolType Type { get; }

            /// <summary>
            /// All symbols must store a copy of the raw text from which the symbol is derived
            /// </summary>
            string RawText { get; }

            /// <summary>
            /// The main symbol function, called when generating a new string from the TextGen singleton.
            /// </summary>
            /// <param name="depth">The current depth of the search</param>
            /// <returns>A string expansion of the content of the symbol</returns>
            string Expand(int depth);
        }
    }
}