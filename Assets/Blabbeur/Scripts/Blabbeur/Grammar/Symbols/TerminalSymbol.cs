namespace Blabbeur
{
    namespace Grammar
    {
        /// <summary>
        /// A terminal symbol is the endpoint of generation, ie. it only refers to the value in it and nothing else.
        /// </summary>
        public class TerminalSymbol : ISymbol
        {
            /// <summary>
            /// The symbol type
            /// </summary>
            public SymbolType Type => SymbolType.TERMINAL;

            /// <summary>
            /// A terminal symbol may have one of many values, which is stored using a type value
            /// </summary>
            private TypedValue text;

            public TypedValue TextObject { get => text; }

            /// <summary>
            /// The raw text refers to the string of the value stored in the symbol
            /// </summary>
            public string RawText { get => text.GetString(); }

            /// <summary>
            /// Check if the store value is numerical, mainly used if the symbol is used in a function calculation
            /// </summary>
            public bool IsNumerical { get => text.IsNumerical(); }

            /// <summary>
            /// Terminal Symbol constructor using strings as the input.
            /// </summary>
            /// <param name="_text">The parsed text for the terminal symbol</param>
            public TerminalSymbol(string _text) => text = new TypedValue(ValueType.STRING, _text);

            /// <summary>
            /// Terminal symbol constructor with a particular object as the input
            /// </summary>
            /// <param name="_type">The value type of the value being passed</param>
            /// <param name="_value">The object itself</param>
            public TerminalSymbol(ValueType _type, object _value) => text = new TypedValue(_type, _value);

            /// <summary>
            /// Expansion of a terminal symbol always returns its raw text.
            /// </summary>
            /// <param name="depth">The current depth of the expansion</param>
            /// <returns>The string representation of the stored object</returns>
            public string Expand(int depth) => RawText;
        }
    }
}