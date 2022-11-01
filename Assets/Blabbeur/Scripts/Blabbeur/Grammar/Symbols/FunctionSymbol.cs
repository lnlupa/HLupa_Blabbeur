namespace Blabbeur
{
    namespace Grammar
    {
        /// <summary>
        /// A function symbol is not typically expanded along with the grammar, rather it is a condition to be evaluated to see if a production rule is available.
        /// It can still, however, be expanded returning a textual expansion of the function.
        /// </summary>
        public class FunctionSymbol : ISymbol
        {
            /// <summary>
            /// The symbol type
            /// </summary>
            public SymbolType Type => SymbolType.FUNCTION;

            /// <summary>
            /// The raw text
            /// </summary>
            private string rawText;

            public string RawText => rawText;

            /// <summary>
            /// The left hand and right hand side symbols of the function
            /// </summary>
            private ISymbol lhs, rhs;

            /// <summary>
            /// The operator being used in the function
            /// </summary>
            private Operator operation;

            /// <summary>
            /// The function symbol constructor requires the function to be fully processed beforehand.
            /// </summary>
            /// <param name="_lhs">The lefthand side of the function</param>
            /// <param name="_op">The operator being called</param>
            /// <param name="_rhs">The righthandside of the function</param>
            /// <param name="_raw">The raw text of the function</param>
            public FunctionSymbol(ISymbol _lhs, Operator _op, ISymbol _rhs, string _raw)
            {
                lhs = _lhs;
                rhs = _rhs;
                operation = _op;
                rawText = _raw;
            }

            /// <summary>
            /// Expanding the function symbol returns a textual representation of the function and its result.
            /// </summary>
            /// <param name="depth">The current depth</param>
            /// <returns>The function as a string</returns>
            public string Expand(int depth)
            {
                return string.Format("[{0}: [{1} {2}] {3} [{4} {5}] = {6} ]", Type, lhs.Type, lhs.Expand(depth), operation, rhs.Type, rhs.Expand(depth), Evaluate());
                //Evaluate().ToString();
            }

            #region Runtime Function Processing

            /// <summary>
            /// Evaluate whether or not a function symbol is true or false.
            /// </summary>
            /// <returns>The boolean result of evaluation</returns>
            public bool Evaluate()
            {
                //Evaluation splits based upon the operator required and calls the appropriate function
                switch (operation)
                {
                    case Operator.AND:
                        return And();

                    case Operator.OR:
                        return Or();

                    case Operator.LESSTHAN:
                        return LessThan(false);

                    case Operator.LESSTHANEQUAL:
                        return LessThan(true);

                    case Operator.GREATERTHAN:
                        return GreaterThan(false);

                    case Operator.GREATERTHANEQUAL:
                        return GreaterThan(true);

                    case Operator.EQUALS:
                        return Equals();

                    case Operator.NOTEQUALS:
                        return !Equals();
                }

                return false;
            }

            /// <summary>
            /// Gets the value of a particular symbol as a float.
            /// </summary>
            /// <param name="s">The symbol being checked.</param>
            /// <returns>The value as a float.</returns>
            private float GetValue(ISymbol s)
            {
                if (s.Type == SymbolType.TERMINAL)
                {
                    return ((TerminalSymbol)s).TextObject.GetFloat();
                }
                    
                else if (s.Type == SymbolType.REFERENCE)
                {
                    return ((ReferenceSymbol)s).RequestFloat();
                }
                    
                else
                    throw new System.Exception(string.Format("Invalid attempt to request a float from symbol type {0}", s.Type));
            }

            /// <summary>
            /// Processes the greater than function
            /// </summary>
            /// <param name="equals">If we should also return true if the lhs and rhs are equal</param>
            /// <returns>lhs >[=] rhs </returns>
            private bool GreaterThan(bool equals) => equals ? System.Single.Parse(lhs.Expand(0)) >= System.Single.Parse(rhs.Expand(0)) : System.Single.Parse(lhs.Expand(0)) > System.Single.Parse(rhs.Expand(0));

            /// <summary>
            /// Processes the less than function
            /// </summary>
            /// <param name="equals">If we should also return true if the lhs and rhs are equal</param>
            /// <returns>lhs <[=] rhs </returns>
            private bool LessThan(bool equals) => equals ? !GreaterThan(false) : !GreaterThan(true);

            /// <summary>
            /// Processes the equals than function
            /// </summary>
            /// <returns>lhs == rhs</returns>
            private bool Equals() {
                //is this numerical?
                float lhs_num;
                float rhs_num;
                if (System.Single.TryParse(lhs.Expand(0), out lhs_num))
                {
                    if (System.Single.TryParse(rhs.Expand(0), out rhs_num))
                    {
                        return lhs_num == rhs_num;
                    }
                }
                return lhs.Expand(0) == rhs.Expand(0);
            }

            /// <summary>
            /// Processes the or function. For these functions, both the lhs and rhs must be function symbols.
            /// </summary>
            /// <returns>lhs || rhs </returns>
            private bool Or()
            {
                if (lhs.Type == SymbolType.FUNCTION && rhs.Type == SymbolType.FUNCTION)
                    return ((FunctionSymbol)lhs).Evaluate() || ((FunctionSymbol)rhs).Evaluate();
                return false;
            }

            /// <summary>
            /// Processes the and function. For these functions, both the lhs and rhs must be function symbols.
            /// </summary>
            /// <returns>lhs && rhs</returns>
            private bool And()
            {
                if (lhs.Type == SymbolType.FUNCTION && rhs.Type == SymbolType.FUNCTION)
                    return ((FunctionSymbol)lhs).Evaluate() && ((FunctionSymbol)rhs).Evaluate();
                return false;
            }

            #endregion Runtime Function Processing
        }
    }
}