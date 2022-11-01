using Blabbeur.Grammar;
using Blabbeur.Objects;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization.Json;

namespace Blabbeur
{
    /// <summary>
    /// The Blabbeur parser takes in raw text, and converts it into a Blabbeur Grammar, but it can also provide some other text parsing functionality
    /// </summary>
    public static class Parser
    {
        #region Constants

        private const char SYMBOLOPENCHARACTER = '[';
        private const char SYMBOLCLOSECHARACTER = ']';

        private const char FUNCTIONOPENCHARACTER = '{';
        private const char FUNCTIONCLOSECHARACTER = '}';

        private const char REFERENCEOPENCHARACTER = '<';
        private const char REFERENCECLOSECHARACTER = '>';

        private const char LINEENDING = '\n';
        private const char SYMBOLSEPARATOR = ';';

        private const char PRODUCTIONRULESEPARATOR = ':';
        private const string COMMENTS = "//";

        private const char STRINGSYMBOL = '"';
        private const string TRUESYMBOL = "true";
        private const string FALSESYMBOL = "false";

        /// <summary>
        /// A dictionary defining all the operators possible in function symbols
        /// </summary>
        private static readonly Dictionary<Operator, string> OPERATORS = new Dictionary<Operator, string>
        {
            [Operator.AND] = "&&",
            [Operator.EQUALS] = "==",
            [Operator.GREATERTHAN] = ">",
            [Operator.GREATERTHANEQUAL] = ">=",
            [Operator.LESSTHAN] = "<",
            [Operator.LESSTHANEQUAL] = "<=",
            [Operator.NOTEQUALS] = "!=",
            [Operator.OR] = "||"
        };

        #endregion Constants

        #region Text Parsing

        /// <summary>
        /// Load a grammar from a body of text
        /// </summary>
        /// <param name="lines">The body of text to parse</param>
        /// <returns>A Blabbeur Grammar</returns>
        public static BlabbeurGrammar LoadBlabbeurGrammar(string lines) => LoadBlabbeurGrammar(ProcessRawText(lines));

        /// <summary>
        /// Load a grammar from a list of strings
        /// </summary>
        /// <param name="lines">The list of strings</param>
        /// <returns>A blabbeur grammar</returns>
        public static BlabbeurGrammar LoadBlabbeurGrammar(List<string> lines)
        {
            if (lines.Count < 0) throw new System.Exception("Loading Error: No Suitable Production Rules Found.");
            else
            {
                //Set up the defaults for the blabbeur grammar
                FlagValue<string> name = new FlagValue<string>("", false);
                Dictionary<string, List<ProductionRule>> productionRules = new Dictionary<string, List<ProductionRule>>();

                //Now process each line
                for (int i = 0; i < lines.Count; i++)
                {
                    //Only process production rules (ignoring comments, spaces, etc.)
                    if (lines[i].Contains(PRODUCTIONRULESEPARATOR.ToString()))
                    {
                        string[] line = lines[i].Split(PRODUCTIONRULESEPARATOR);
                        string lhs = CleanString(line[0]);
                        string rhs = CleanString(line[1]);
                        if (!string.IsNullOrEmpty(lhs) && !string.IsNullOrEmpty(rhs))
                        {
                            //The first production rule is the name and root of the blabbeur grammar
                            if (!name.flag) name.Set(lhs);

                            //Process the individual expansions of each production rule
                            ProcessProductionRules(ref productionRules, lhs, rhs);
                        }
                    }
                }

                return new BlabbeurGrammar(name.value, name.value, productionRules);
            }
        }

        /// <summary>
        /// Splits a production rule into different expansions and adds those to a new production rule
        /// </summary>
        /// <param name="dict">The production rule dictionary used by the grammar</param>
        /// <param name="lhs">The name of the production rule</param>
        /// <param name="rhs">The raw, right hand side of the text</param>
        private static void ProcessProductionRules(ref Dictionary<string, List<ProductionRule>> dict, string lhs, string rhs)
        {
            if (!dict.ContainsKey(lhs)) dict[lhs] = new List<ProductionRule>();

            //Split the right hand side according to the separator
            string[] _rhs = rhs.Split(SYMBOLSEPARATOR);
            for (int j = 0; j < _rhs.Length; j++)
            {
                //Clean and then process each specific expansion
                string rhs_clean = CleanString(_rhs[j]);
                if (!string.IsNullOrEmpty(rhs_clean))
                    dict[lhs].Add(ProcessProductionRule(rhs_clean));
            }
        }

        /// <summary>
        /// Process a production rule into a list of its symbols
        /// </summary>
        /// <param name="line">The raw text to be processed</param>
        /// <returns>A production rule</returns>
        private static ProductionRule ProcessProductionRule(string line)
        {
            List<ISymbol> symbols = new List<ISymbol>();

            string activeText = "";
            SymbolType activeType = SymbolType.TERMINAL;

            //The possbile precondition of the rule
            FlagValue<FunctionSymbol> precondition = new FlagValue<FunctionSymbol>(null, false);

            //Processing proceeds left to right, keeping track of the current symbol being read
            //When a new close symbol is read, the previous symbol is completed and added to the rule
            for (int i = 0; i < line.Length; i++)
            {
                if (ProcessSymbol(line[i], activeType))
                {
                    switch (line[i])
                    {
                        case SYMBOLOPENCHARACTER:
                            if (!string.IsNullOrEmpty(activeText))
                                symbols.Add(GenerateSymbol(activeType, activeText));
                            activeText = "";
                            activeType = SymbolType.RULE;
                            break;

                        case SYMBOLCLOSECHARACTER:
                            if (!string.IsNullOrEmpty(activeText))
                                symbols.Add(GenerateSymbol(activeType, activeText));
                            activeText = "";
                            activeType = SymbolType.TERMINAL;
                            break;

                        case FUNCTIONOPENCHARACTER:
                            if (!string.IsNullOrEmpty(activeText))
                                symbols.Add(GenerateSymbol(activeType, activeText));
                            activeText = "";
                            activeType = SymbolType.FUNCTION;
                            break;

                        case FUNCTIONCLOSECHARACTER:
                            if (!string.IsNullOrEmpty(activeText))
                                precondition.Set(ProcessFunction(activeText));
                            activeText = "";
                            activeType = SymbolType.TERMINAL;
                            break;

                        case REFERENCEOPENCHARACTER:
                            if (!string.IsNullOrEmpty(activeText))
                                symbols.Add(GenerateSymbol(activeType, activeText));
                            activeText = "";
                            activeType = SymbolType.REFERENCE;
                            break;

                        case REFERENCECLOSECHARACTER:
                            if (!string.IsNullOrEmpty(activeText))
                                symbols.Add(GenerateSymbol(activeType, activeText));
                            activeText = "";
                            activeType = SymbolType.TERMINAL;
                            break;

                        default:
                            activeText += line[i];
                            break;
                    }
                }
                else { activeText += line[i]; }
            }
            if (!string.IsNullOrEmpty(activeText))
                symbols.Add(GenerateSymbol(activeType, activeText));

            return precondition.flag ? new ProductionRule(symbols, precondition.value) : new ProductionRule(symbols);
        }

        /// <summary>
        /// Check if we need to process the current character being read
        /// </summary>
        /// <param name="c">The character being read</param>
        /// <param name="t">The active symbol being created</param>
        /// <returns>True if the current character means we are now processing a new object</returns>
        private static bool ProcessSymbol(char c, SymbolType t)
        {
            switch (c)
            {
                case FUNCTIONOPENCHARACTER:
                    return t == SymbolType.TERMINAL;

                case FUNCTIONCLOSECHARACTER:
                    return t == SymbolType.FUNCTION;

                case REFERENCEOPENCHARACTER:
                    return t == SymbolType.TERMINAL;

                case REFERENCECLOSECHARACTER:
                    return t == SymbolType.REFERENCE;

                case SYMBOLOPENCHARACTER:
                    return t == SymbolType.TERMINAL;

                case SYMBOLCLOSECHARACTER:
                    return t == SymbolType.RULE;

                default:
                    return true;
            }
        }

        /// <summary>
        /// Process raw text into a particular symbol
        /// </summary>
        /// <param name="type">The type of symbol to generate</param>
        /// <param name="text">The raw text</param>
        /// <returns>A new ISymbol of the type provided</returns>
        private static ISymbol GenerateSymbol(SymbolType type, string text)
        {
            switch (type)
            {
                case SymbolType.FUNCTION:
                    return ProcessFunction(text);

                case SymbolType.REFERENCE:
                    return new ReferenceSymbol(text);

                case SymbolType.RULE:
                    return new RuleSymbol(text);

                case SymbolType.TERMINAL:
                    return new TerminalSymbol(text);
            }
            return null;
        }

        #endregion Text Parsing

        #region Function Parsing

        /// <summary>
        /// Process raw text into a function
        /// </summary>
        /// <param name="text">The raw text to be processed</param>
        /// <returns>The processed function symbol</returns>
        public static FunctionSymbol ProcessFunction(string text)
        {
            //We check for operators in a specific order. Mainly, we need to evaluate and/or with higher priority than numerical checks.
            Operator[] order = new Operator[8] { Operator.AND, Operator.OR, Operator.EQUALS, Operator.NOTEQUALS, Operator.GREATERTHANEQUAL, Operator.GREATERTHAN, Operator.LESSTHANEQUAL, Operator.LESSTHAN };

            //Move through the text, and split when we encounter the current operator in the order
            for (int i = 0; i < order.Length; i++)
                if (text.Contains(OPERATORS[order[i]]))
                {
                    //Split the string on the operator
                    string[] splitString = text.Split(new string[1] { OPERATORS[order[i]] }, System.StringSplitOptions.RemoveEmptyEntries);

                    //Recursively call any subfunctions which need to be processed
                    return new FunctionSymbol(ProcessSubFunction(splitString[0]), order[i], ProcessSubFunction(splitString[1]), text);
                }

            return null;
        }

        /// <summary>
        /// Process a subfunction of the function, recursively evaluating sub functions if necessary
        /// </summary>
        /// <param name="text">The text to be processed</param>
        /// <returns>The proccessed symbol</returns>
        public static ISymbol ProcessSubFunction(string text)
        {
            text = CleanString(text);
            //Order is important here
            if (IsString(text)) return new TerminalSymbol(ValueType.STRING, text.Replace(STRINGSYMBOL.ToString(), ""));
            else if (IsBool(text)) return new TerminalSymbol(ValueType.BOOL, text == TRUESYMBOL ? true : false);
            else if (IsFloat(text)) return new TerminalSymbol(ValueType.FLOAT, float.Parse(text));
            else if (IsDouble(text)) return new TerminalSymbol(ValueType.DOUBLE, double.Parse(text));
            else if (IsInt(text)) return new TerminalSymbol(ValueType.INTEGER, int.Parse(text));
            else if (IsFunction(text)) return ProcessFunction(text);
            else return new ReferenceSymbol(text);
        }

        /// <summary>
        /// Check if a string is bracketed in the string symbol
        /// </summary>
        /// <param name="text">The text to check</param>
        /// <returns>True if the text is a string</returns>
        private static bool IsString(string text) => First(text).Equals(STRINGSYMBOL) && Last(text).Equals(STRINGSYMBOL);

        /// <summary>
        /// Check if a string is meant to be a boolean
        /// </summary>
        /// <param name="text">The text to check</param>
        /// <returns>True if the text is either the true or false symbol</returns>
        private static bool IsBool(string text) => text.Equals(TRUESYMBOL) || text.Equals(FALSESYMBOL);

        /// <summary>
        /// Check if a string is a function
        /// </summary>
        /// <param name="text">The text to check</param>
        /// <returns>True if the text has one of the operators we're searching for</returns>
        private static bool IsFunction(string text)
        {
            foreach (string op in OPERATORS.Values)
                if (text.Contains(op)) return true;
            return false;
        }

        /// <summary>
        /// Check if a string is an integer
        /// </summary>
        /// <param name="text">The text to check</param>
        /// <returns>True if the text can be processed into an integer</returns>
        private static bool IsInt(string text) => int.TryParse(text, out int i);

        /// <summary>
        /// Check if a string is a float
        /// </summary>
        /// <param name="text">The text to check</param>
        /// <returns>True if the text can be processed into a float</returns>
        private static bool IsFloat(string text) => float.TryParse(text, out float f);

        /// <summary>
        /// Check if a string is a double
        /// </summary>
        /// <param name="text">The text to check</param>
        /// <returns>True if the text can be processed into a double</returns>
        private static bool IsDouble(string text) => double.TryParse(text, out double d);

        #endregion Function Parsing

        #region Value Processing Functions

        /// <summary>
        /// Takes in an object, and returns which valuetype it falls under in blabbeur
        /// </summary>
        /// <param name="value">The object to classify</param>
        /// <returns>The valuetype</returns>
        public static ValueType ClassifyValue(object value)
        {
            if (value is int) return ValueType.INTEGER;
            else if (value is double) return ValueType.DOUBLE;
            else if (value is float) return ValueType.FLOAT;
            else if (value is string) return ValueType.STRING;
            else if (value is BlabbeurObject) return ValueType.OBJECT;
            else if (value is bool) return ValueType.BOOL;
            else return ValueType.INVALID;
        }

        #endregion Value Processing Functions

        #region Basic String Processing Functions

        /// <summary>
        /// Get the first character in a string
        /// </summary>
        /// <param name="text">the string</param>
        /// <returns>the first character</returns>
        private static char First(string text) => text[0];

        /// <summary>
        /// Get the last character in a string
        /// </summary>
        /// <param name="text">the string</param>
        /// <returns>the last character</returns>
        private static char Last(string text) => text[text.Length - 1];

        /// <summary>
        /// Check if a given line should not be considered part of the grammar
        /// </summary>
        /// <param name="line">The line to be processed</param>
        /// <returns>True if the line is a comment, or is empty</returns>
        private static bool ignoreLine(string line)
        {
            if (line.StartsWith(COMMENTS)) return true;
            else if (string.IsNullOrEmpty(line)) return true;
            return false;
        }

        /// <summary>
        /// Clean whitespace and line endings from text
        /// </summary>
        /// <param name="text">The text to clean</param>
        /// <returns>The cleaned text</returns>
        private static string CleanString(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            text = text.Replace('\n', ' ').Replace('\t', ' ').Trim();
            return text;
        }

        /// <summary>
        /// Process a raw body of text into lines on the given lineending symbol
        /// </summary>
        /// <param name="text">The raw text</param>
        /// <returns>The processed list of text</returns>
        private static List<string> ProcessRawText(string text)
        {
            List<string> linesProcessed = new List<string>();
            string[] lines = text.Split(LINEENDING);

            string activeString = "";
            //Go through each line
            for (int i = 0; i < lines.Length; i++)
            {
                //If its a comment or empty, ignore it.
                lines[i] = CleanString(lines[i]);
                if (!ignoreLine(lines[i]))
                {

                    //Otherwise, check if it contains a the symbol char (':')
                    if (lines[i].Contains(PRODUCTIONRULESEPARATOR.ToString()))
                    {
                        //If so, add the active string to the list
                        if (!string.IsNullOrEmpty(activeString))
                            linesProcessed.Add(activeString);

                        //Set the new active string
                        activeString = lines[i];
                    }
                    else
                    {
                        //Otherwise, add the lines to the active string
                        activeString = string.Format("{0}{1}", activeString, lines[i]);
                    }
                    //linesProcessed.Add(lines[i]);
                }
            }

            if (!string.IsNullOrEmpty(activeString))
                linesProcessed.Add(activeString);

            /*for (int i = 0; i < linesProcessed.Count; i++)
                UnityEngine.Debug.Log(linesProcessed[i]);*/

            return linesProcessed;
        }

        #endregion Basic String Processing Functions
    }
}