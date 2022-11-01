using System;

/// <summary>
/// Implements a flagging system whereby an object may be flagged as true or false. Useful for, e.g. flagging if a file was loaded correctly before reading an object.
///
/// Author: Quinn Kybartas
/// Date: 11/12/20
/// </summary>
/// <typeparam name="T">The type of flagged object to be created</typeparam>
[Serializable]
public class FlagValue<T>
{
    public bool flag; //The value's flag
    public T value; //The value itself

    /// <summary>
    /// Check if the flag of a given flag value is true.
    /// </summary>
    /// <param name="q">The value to check</param>
    /// <returns>True if the flag is true</returns>
    public static bool operator true(FlagValue<T> q) => q.flag;

    /// <summary>
    /// Check if the flag of a given flagvalue is false
    /// </summary>
    /// <param name="q">The value to check</param>
    /// <returns>True if the flag is false</returns>
    public static bool operator false(FlagValue<T> q) => q.flag == false;

    /// <summary>
    /// Flagvalue constructor
    /// </summary>
    /// <param name="_value">The value we are setting</param>
    /// <param name="_flag">The flag for the value (defaults to true)</param>
    public FlagValue(T _value, bool _flag = true) => Set(_value, _flag);

    /// <summary>
    /// Sets the values of the flagvalue.
    /// </summary>
    /// <param name="_value">The value we are setting</param>
    /// <param name="_flag">The flag for the value (defaults to true)</param>
    public void Set(T _value, bool _flag = true)
    {
        value = _value; flag = _flag;
    }
}