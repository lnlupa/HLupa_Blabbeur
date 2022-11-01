namespace Blabbeur
{
    namespace Objects
    {
        /// <summary>
        /// A Property is a typed value that is also assigned an identifier, eg. Age: 35 [Integer]
        /// </summary>
        public struct Property
        {
            /// <summary>
            /// The identifier of the property
            /// </summary>
            private string name;

            public string Name { get => name; }

            /// <summary>
            /// The TypedValue the property represents
            /// </summary>
            private TypedValue value;

            public object Value { get => value.Value; }
            public ValueType Type { get => value.Type; }

            /// <summary>
            /// Construct a new property from a given identifier and object
            /// </summary>
            /// <param name="_name">The identifier</param>
            /// <param name="_value">The object</param>
            public Property(string _name, object _value)
            {
                name = _name;
                value = new TypedValue(_value);
            }

            /// <summary>
            /// Construct a new property from a given identifier, object, and object type
            /// </summary>
            /// <param name="_name">The identifier</param>
            /// <param name="_value">The object</param>
            /// <param name="_type">The object type</param>
            public Property(string _name, object _value, ValueType _type)
            {
                name = _name;
                value = new TypedValue(_type, _value);
            }

            /// <summary>
            /// Request the typed value as an int.
            /// </summary>
            /// <returns>An integer</returns>
            public int GetInt() => value.GetInt();

            /// <summary>
            /// Request the typed value as a double.
            /// </summary>
            /// <returns>A double</returns>
            public double GetDouble() => value.GetDouble();

            /// <summary>
            /// Request the typed value as a string.
            /// </summary>
            /// <returns>A string</returns>
            public string GetString() => value.GetString();

            /// <summary>
            /// Request the typed value as a bool.
            /// </summary>
            /// <returns>A boolean</returns>
            public bool GetBool() => value.GetBool();

            /// <summary>
            /// Request the typed value as a bool.
            /// </summary>
            /// <returns>A boolean</returns>
            public float GetFloat() => value.GetFloat();

            /// <summary>
            /// Request the typed value as a BlabbeurObject.
            /// </summary>
            /// <returns>A BlabbeurObject</returns>
            public BlabbeurObject GetBlabbeurObject() => value.GetBlabbeurObject();

            /// <summary>
            /// Override of tostring function
            /// </summary>
            /// <returns>Property in the form of name: value[type]</returns>
            public override string ToString() => string.Format("{0}: {1} [{2}]", name, Value, Type);
        }
    }
}