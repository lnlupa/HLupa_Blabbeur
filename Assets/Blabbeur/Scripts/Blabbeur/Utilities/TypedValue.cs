using Blabbeur.Objects;

namespace Blabbeur
{
    /// <summary>
    /// A type value is a class which stores the type information about an object. Removing the need to use typeof() arguments
    /// </summary>
    public class TypedValue
    {
        /// <summary>
        /// The type of the value
        /// </summary>
        private ValueType type;

        public ValueType Type { get => type; }

        /// <summary>
        /// The typed value object
        /// </summary>
        private object tValue;

        public object Value { get => tValue; }

        /// <summary>
        /// Constructor for a type value.
        /// </summary>
        /// <param name="_type">The type of the object</param>
        /// <param name="_value">The object itself</param>
        public TypedValue(ValueType _type, object _value)
        {
            type = _type;
            tValue = _value;
        }

        /// <summary>
        /// Constructor for a typed value
        /// </summary>
        /// <param name="_value">The object</param>
        public TypedValue(object _value)
        {
            //Classify the type of the object using the parser
            type = Parser.ClassifyValue(_value);
            tValue = _value;
        }

        /// <summary>
        /// Check if the value of the type is numerical
        /// </summary>
        /// <param name="t">The type</param>
        /// <returns>If the type is numerical</returns>
        public bool IsNumerical(ValueType t) => t == ValueType.DOUBLE || t == ValueType.FLOAT || t == ValueType.INTEGER;

        /// <summary>
        /// Check if this typed value is numerical
        /// </summary>
        /// <returns>True if the type is numerical</returns>
        public bool IsNumerical() => IsNumerical(type);

        /// <summary>
        /// Check if this type can be requested as a different type and still work
        /// </summary>
        /// <param name="otherType"></param>
        /// <returns></returns>
        public bool CanRepresent(ValueType otherType)
        {
            if (otherType == ValueType.STRING) return true; //Every object has a toString() function.
            else if (type == ValueType.INVALID || otherType == ValueType.INVALID) return false; //Invalid objects can't be anything
            else if (IsNumerical() && IsNumerical(otherType)) return true; //Numerical objects can be cast as other numerical objects
            else if (type == otherType) return true; //If the types match then there's no issue
            return false;
        }

        /// <summary>
        /// Return this type as a abool
        /// </summary>
        /// <returns>The value as a bool</returns>
        public bool GetBool()
        {
            if (type == ValueType.BOOL) return (bool)tValue;
            else throw CastError(ValueType.BOOL);
        }

        /// <summary>
        /// Return this type as an integer
        /// </summary>
        /// <returns>The value as an integer</returns>
        public int GetInt()
        {
            //Doubles, floats and integer can all be intercast
            if (IsNumerical())
            {
                switch (type)
                {
                    case ValueType.DOUBLE:
                        return (int)(double)tValue;

                    case ValueType.FLOAT:
                        return (int)(float)tValue;

                    case ValueType.INTEGER:
                        return (int)tValue;

                    default:
                        throw NumericalError();
                }
            }
            else throw NumericalError();
        }

        /// <summary>
        /// Return this type as a double
        /// </summary>
        /// <returns>The value as a double</returns>
        public double GetDouble()
        {
            //Doubles, floats and integer can all be intercast
            if (IsNumerical())
            {
                switch (type)
                {
                    case ValueType.DOUBLE:
                        return (double)tValue;

                    case ValueType.FLOAT:
                        return (double)(float)tValue;

                    case ValueType.INTEGER:
                        return (double)(int)tValue;

                    default:
                        throw NumericalError();
                }
            }
            else throw NumericalError();
        }

        /// <summary>
        /// Return this type as a float
        /// </summary>
        /// <returns>The value as a float</returns>
        public float GetFloat()
        {
            //Doubles, floats and integer can all be intercast
            if (IsNumerical())
            {
                switch (type)
                {
                    case ValueType.DOUBLE:
                        return (float)(double)tValue;

                    case ValueType.FLOAT:
                        return (float)tValue;

                    case ValueType.INTEGER:
                        return (float)(int)tValue;

                    default:
                        throw NumericalError();
                }
            }
            else throw NumericalError();
        }

        /// <summary>
        /// Return this type as a string
        /// </summary>
        /// <returns>The value as a string</returns>
        public string GetString() => tValue.ToString();

        /// <summary>
        /// Return this type as a Blabbeur Object
        /// </summary>
        /// <returns>The value as a Blabbeur Object</returns>
        public BlabbeurObject GetBlabbeurObject()
        {
            if (type == ValueType.OBJECT) return (BlabbeurObject)tValue;
            else throw CastError(ValueType.OBJECT);
        }

        /// <summary>
        /// Throw an error if trying to access a non-numerical type as a number
        /// </summary>
        /// <returns>Throws a system exception</returns>
        private System.Exception NumericalError() => new System.Exception("Attempting to cast a non-numerical value to a numerical value!");

        /// <summary>
        /// Throw an error if an impossible cast is attempted
        /// </summary>
        /// <param name="t">The type requested</param>
        /// <returns>Throws a system exception</returns>
        private System.Exception CastError(ValueType t) => new System.Exception(string.Format("Type Value cast error! Expected {0}, Actual {1}", t, type));
    }
}