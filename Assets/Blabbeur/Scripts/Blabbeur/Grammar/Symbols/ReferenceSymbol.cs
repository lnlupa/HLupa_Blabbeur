using Blabbeur.Objects;

namespace Blabbeur
{
    namespace Grammar
    {
        /// <summary>
        /// A reference symbol contains a reference to either a local or global object
        /// </summary>
        public class ReferenceSymbol : ISymbol
        {
            /// <summary>
            /// The symbol type.
            /// </summary>
            public SymbolType Type => SymbolType.REFERENCE;

            /// <summary>
            /// The raw text is the full object reference.
            /// </summary>
            private string rawText;

            public string RawText => rawText;

            /// <summary>
            /// The processed text stores the list of references
            /// </summary>
            public string[] processedText;

            /// <summary>
            /// The reference constructor takes the full string as reference, and decomposes it down to its processed text.
            /// </summary>
            /// <param name="_text">The full reference text</param>
            public ReferenceSymbol(string _text)
            {
                rawText = _text;
                //Since refs are delineated by '.', processing is simply a case of splitting the text on this character
                processedText = _text.Split('.');
            }

            /// <summary>
            /// Useful quick function to get the actual name of the variable being searched for
            /// </summary>
            private string Last => processedText[processedText.Length - 1];

            /// <summary>
            /// Get the final reference object, from which we request the variable
            /// </summary>
            private BlabbeurObject ReferencedObject => TextGen.RequestObject(processedText);

            /// <summary>
            /// The expansion will always get the final variable as a string
            /// </summary>
            /// <param name="depth">The current depth</param>
            /// <returns>The referenced variable as a string</returns>
            public string Expand(int depth) => RequestString();

            /// <summary>
            /// Request the string value of the referenced object
            /// </summary>
            /// <returns>The object value as a string</returns>
            public string RequestString() => ReferencedObject.RequestString(Last);

            /// <summary>
            /// Request the boolean value of the referenced object
            /// </summary>
            /// <returns>The boolean object referred to</returns>
            public bool RequestBool() => ReferencedObject.RequestBool(Last);

            /// <summary>
            /// Request the float value of the referenced object
            /// </summary>
            /// <returns>The float object referred to</returns>
            public float RequestFloat() => ReferencedObject.RequestFloat(Last);

            /// <summary>
            /// Request the integer value of the referenced object
            /// </summary>
            /// <returns>The integer object referred to</returns>
            public int RequestInt() => ReferencedObject.RequestInt(Last);

            /// <summary>
            /// Request the double value of the referenced object
            /// </summary>
            /// <returns>The double object referred to</returns>
            public double RequestDouble() => ReferencedObject.RequestDouble(Last);
        }
    }
}