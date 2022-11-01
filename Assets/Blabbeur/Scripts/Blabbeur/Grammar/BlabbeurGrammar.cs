using System;
using System.Collections.Generic;

namespace Blabbeur
{
    namespace Grammar
    {
        /// <summary>
        /// Implements a grammar to be used within the blabbeur system
        /// </summary>
        public class BlabbeurGrammar
        {
            /// <summary>
            /// The maximum depth to check when expanding before aborting
            /// </summary>
            private const int MAX_DEPTH = 10;

            /// <summary>
            /// An error rule to be used for errors
            /// </summary>
            private ProductionRule ERROR = new ProductionRule(new List<ISymbol> { new TerminalSymbol("") });

            /// <summary>
            /// The dictionary of production rules, mapped to the symbol on the lhs.
            /// </summary>
            private Dictionary<string, List<ProductionRule>> productionRules;

            public Dictionary<string, List<ProductionRule>> ProductionRules { get { return productionRules; } }

            /// <summary>
            /// The name of the context free grammar
            /// </summary>
            private string name;

            public string Name { get { return name; } }

            /// <summary>
            /// The root production rule of the grammar
            /// </summary>
            private string root;

            /// <summary>
            /// We keep an internal randomizer for all random choices
            /// </summary>
            private Random random;

            /// <summary>
            /// The grammar constructor, takes in the name and root ids, all rules, and a randomizer seed
            /// </summary>
            /// <param name="_name">The name of the grammar</param>
            /// <param name="_root">The root production rule being used</param>
            /// <param name="_rules">A list of production rules to be used by the system</param>
            /// <param name="_seed">[Optional] A specific randomizer seed</param>
            public BlabbeurGrammar(string _name, string _root, Dictionary<string, List<ProductionRule>> _rules, int _seed = 0)
            {
                name = _name;
                root = _root;
                productionRules = _rules;

                if (_seed != 0) random = new Random(_seed);
                else random = new Random();
            }

            //Attempts to expand a production rule, ensuring any preconditons are met
            private List<ProductionRule> GetAvailableExpansions(string lhs)
            {
                List<ProductionRule> valid = new List<ProductionRule>();

                //Look through the rules
                if (productionRules.ContainsKey(lhs))
                    for (int i = 0; i < productionRules[lhs].Count; i++)
                        //Evaluate the function symbols, if necessary
                        if (productionRules[lhs][i].Available)
                            valid.Add(productionRules[lhs][i]);

                return valid;
            }

            /// <summary>
            /// Choose a random production rule to apply for a nonterminal symbol
            /// </summary>
            /// <param name="lhs">The left hand side of the rule</param>
            /// <returns>One of the possible production rules with the given lhs</returns>
            private ProductionRule GetExpansion(string lhs)
            {
                List<ProductionRule> rules = GetAvailableExpansions(lhs);

                if (rules.Count > 0)
                {
                    int selected = random.Next(0, rules.Count);
                    ProductionRule chosen = rules[selected];
                    return chosen;
                }
                else
                {
                    TextGen.LogWarning(string.Format("Production Rule {0}: Has no valid expansions.", lhs));
                    UnityEngine.Debug.Log(string.Format("Production Rule {0}: Has no valid expansions.", lhs));
                    return ERROR; //Return an error if we don't have any matching rules
                }
            }

            /// <summary>
            /// Remove any instance of two spaces in a row, this is a fairly common problem in grammar parsing.
            /// </summary>
            /// <param name="text">The line to be cleaned</param>
            /// <returns>A line cleaned of unneccesary characters</returns>
            private string RemoveDoubleSpaces(string text) => text.Replace("  ", " ");

            /// <summary>
            /// Expand the grammar from its root symbol
            /// </summary>
            /// <returns>The string expansion</returns>
            public string Expand() => Expand(root, 0);

            /// <summary>
            /// Expand the grammar from a provided symbol
            /// </summary>
            /// <param name="symbol">The symbol to be expanded</param>
            /// <returns>The resulting line of text</returns>
            public string Expand(string symbol) => Expand(symbol, 0);

            /// <summary>
            /// Recursive function to expand a symbol down to a specific depth
            /// </summary>
            /// <param name="symbol">The symbol to expand</param>
            /// <param name="depth">The current depth of the expansion</param>
            /// <returns></returns>
            public string Expand(string symbol, int depth)
            {
                if (depth >= MAX_DEPTH) return "<MAX DEPTH EXCEEDED, LAST SYMBOL: " + symbol + "!>";

                if (productionRules.ContainsKey(symbol)) //Make sure we have a production rule for the given symbol
                    return RemoveDoubleSpaces(GetExpansion(symbol).Expand());
                else return "<SYMBOL '" + symbol + "' DOES NOT EXIST!>";
            }

            /// <summary>
            /// Basic error checking for a grammar, returns a string with all noted errors
            /// </summary>
            /// <returns>The errors found in the grammar</returns>
            public string Validate()
            {
                string s = "Validating grammar " + name + ":\n";

                List<string> keys = new List<string>();
                foreach (string key in productionRules.Keys) keys.Add(key);

                //Look for missing symbols
                foreach (List<ProductionRule> lrules in productionRules.Values)
                {
                    foreach (ProductionRule rule in lrules)
                    {
                        foreach (ISymbol symb in rule.Symbols)
                        {
                            if ((symb.Type == SymbolType.RULE) && !keys.Contains(symb.RawText))
                            {
                                s += "\tSymbol '" + symb.RawText + "' does not exist!\n";
                            }
                        }
                    }
                }
                return s;
            }
        }
    }
}