/// <summary>
/// A collection of all public enumerators from the Blabbeur system
/// </summary>
namespace Blabbeur
{
    /// <summary>
    /// Classification enum for the type of operator being stored in a function.
    /// </summary>
    public enum Operator { AND, OR, EQUALS, NOTEQUALS, LESSTHAN, GREATERTHAN, LESSTHANEQUAL, GREATERTHANEQUAL }

    /// <summary>
    /// All valid value types for BlabbeurObjects, invalid is used to flag an invalid value.
    /// </summary>
    public enum ValueType { STRING, INTEGER, DOUBLE, FLOAT, OBJECT, BOOL, INVALID }

    /// <summary>
    /// All valid symbol types for a Blabbeur grammar
    /// </summary>
    public enum SymbolType { RULE, TERMINAL, FUNCTION, REFERENCE }
}