using System;
using System.Collections.Generic;

namespace Blabbeur
{
    namespace Objects
    {
        /// <summary>
        /// The property dictionary is a BlabbeurObject implemented in the form of a dictionary
        /// </summary>
        public class PropertyDictionary : BlabbeurObject
        {
            /// <summary>
            /// We store the objects as properties in a dictionary
            /// </summary>
            private Dictionary<string, Property> dictionary;

            /// <summary>
            /// The id of the object
            /// </summary>
            private string id;

            public string ID { get => id; set => id = value; }

            /// <summary>
            /// Property Dictionary constructor
            /// </summary>
            /// <param name="_name"></param>
            public PropertyDictionary(string _name)
            {
                dictionary = new Dictionary<string, Property>();
                id = _name;
            }

            /// <summary>
            /// Get an object by name
            /// </summary>
            /// <param name="key">The name of the object</param>
            /// <returns>A property</returns>
            public object this[string key]
            {
                get => GetProperty(key).Value;
                set => Add(key, value);
            }

            /// <summary>
            /// Get an object by name
            /// </summary>
            /// <param name="key">The name of the object</param>
            /// <returns>A property</returns>
            private Property GetProperty(string name)
            {
                if (dictionary.ContainsKey(name)) return dictionary[name];
                else throw new Exception(string.Format("{0} is not found within the property dictionary", name));
            }

            /// <summary>
            /// Add a new property to the dictionary
            /// </summary>
            /// <param name="property">The property to be added</param>
            public void Add(Property property) => dictionary[property.Name] = property;

            /// <summary>
            /// Add a new property to the dictionary
            /// </summary>
            /// <param name="name">The name of the property to be added</param>
            /// <param name="value">The value of the property</param>
            public void Add(string name, object value) => dictionary[name] = new Property(name, value);

            /// <summary>
            /// Add a new property to the dictionary
            /// </summary>
            /// <param name="name">The name of the property</param>
            /// <param name="property">The property to be added</param>
            public void Add(string name, Property property) => dictionary[name] = property;

            /// <summary>
            /// Request a property as a bool
            /// </summary>
            /// <param name="value">The name of the property</param>
            /// <returns>The resulting boolean</returns>
            public bool RequestBool(string value) => dictionary[value].GetBool();

            /// <summary>
            /// Request a property as a double
            /// </summary>
            /// <param name="value">The name of the property</param>
            /// <returns>The resulting double</returns>
            public double RequestDouble(string value) => dictionary[value].GetDouble();

            /// <summary>
            /// Request a property as a float
            /// </summary>
            /// <param name="value">The name of the property</param>
            /// <returns>The resulting float</returns>
            public float RequestFloat(string value) => dictionary[value].GetFloat();

            /// <summary>
            /// Request a property as an integer
            /// </summary>
            /// <param name="value">The name of the property</param>
            /// <returns>The resulting integer</returns>
            public int RequestInt(string value) => dictionary[value].GetInt();

            /// <summary>
            /// Request a property as a BlabbeurObject
            /// </summary>
            /// <param name="value">The name of the property</param>
            /// <returns>The resulting BlabbeurObject</returns>
            public BlabbeurObject RequestObject(string value) => dictionary[value].GetBlabbeurObject();

            /// <summary>
            /// Request a property as a string
            /// </summary>
            /// <param name="value">The name of the property</param>
            /// <returns>The resulting string</returns>
            public string RequestString(string value) => dictionary[value].GetString();

            public override string ToString()
            {
                string s = string.Format("Property Dictionary: {0}\n", ID);
                foreach (string name in dictionary.Keys)
                    s = string.Format("{0}\t{1}\n", s, dictionary[name]);
                return string.Format("<{0}>", s);
            }
        }
    }
}