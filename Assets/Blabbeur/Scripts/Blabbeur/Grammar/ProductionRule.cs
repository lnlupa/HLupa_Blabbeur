using System.Collections.Generic;

namespace Blabbeur
{
    namespace Grammar
    {
        /// <summary>
        /// The right hand side of a production rule, the rule itself will have many possible production rules
        /// </summary>
        public class ProductionRule
        {
            /// <summary>
            /// The sequential list of symbols from the RHS of the rule.
            /// </summary>
            private List<ISymbol> symbols;

            public List<ISymbol> Symbols { get { return symbols; } }

            /// <summary>
            /// A precondition function, is flagged true if a precondition is assigned
            /// </summary>
            private FlagValue<FunctionSymbol> precondition = new FlagValue<FunctionSymbol>(null, false);

            /// <summary>
            /// Accessor the precondition value
            /// </summary>
            public FunctionSymbol Precondition
            {
                get => precondition.value;
                set => precondition.Set(value, true);
            }

            /// <summary>
            /// Check if we have a preconditon
            /// </summary>
            public bool Available => precondition.flag ? precondition.value.Evaluate() : true;

            /// <summary>
            /// Production rule constructor with a provided list of symbols
            /// </summary>
            /// <param name="_symbols">The symbols comprising the production rule</param>
            public ProductionRule(List<ISymbol> _symbols) => symbols = _symbols;

            /// <summary>
            /// Production rule constructor with a provided list of symbols and the precondition
            /// </summary>
            /// <param name="_symbols"></param>
            /// <param name="_precondition"></param>
            public ProductionRule(List<ISymbol> _symbols, FunctionSymbol _precondition)
            {
                symbols = _symbols;
                Precondition = _precondition;
            }

            /// <summary>
            /// Expand this specific production rule
            /// </summary>
            /// <param name="depth">The current depth of the production rule, defaults to zero</param>
            /// <returns>An expanded version of itself</returns>
            public string Expand(int depth = 0)
            {
                string expansion = "";
                depth++; //This expansion counts as an increase in depth

                //Symbols are expanded left to right
                for (int i = 0; i < symbols.Count; i++)
                    expansion += symbols[i].Expand(depth);
                return expansion;
            }
        }
    }
}