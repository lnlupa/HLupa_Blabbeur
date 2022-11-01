using Blabbeur.Objects;
using UnityEngine;

namespace Blabbeur
{
    namespace Unity
    {
        /// <summary>
        /// A unity specific text generation object, implemented as a monobehaviour meaning it can be edited in the editor
        /// </summary>
        public class PropertyBehaviour : MonoBehaviour, BlabbeurObject
        {
            #region Variables and Initialization

            /// <summary>
            /// The name of the object
            /// </summary>
            public string id;

            /// <summary>
            /// A list of all the properties minus the objects
            /// </summary>
            public UnityProperty[] properties;

            /// <summary>
            /// A list of specifically the object, which allows drag/drop functionality in the editor
            /// </summary>
            public UnityBlabbeurObjectProperty[] blabbeurObjectProperties;

            /// <summary>
            /// On Awake functionality
            /// </summary>
            private void Awake()
            {
                //Make the dictionary, and add all the properties.
                dictionary = new PropertyDictionary(id);

                for (int i = 0; i < properties.Length; i++) dictionary.Add(properties[i].name, properties[i].ToProperty());
                for (int i = 0; i < blabbeurObjectProperties.Length; i++) dictionary.Add(blabbeurObjectProperties[i].name, blabbeurObjectProperties[i].ToProperty());
            }

            #endregion Variables and Initialization

            #region Property Dictionary and Object Interface

            /// <summary>
            /// The property behaviour is essentially an interface to a property dictionary
            /// </summary>
            public PropertyDictionary dictionary;

            /// <summary>
            /// The id of the property behaviour
            /// </summary>
            public string ID { get => id; set => id = value; }

            /// <summary>
            /// Request a property as a bool
            /// </summary>
            /// <param name="value">The name of the property</param>
            /// <returns>The resulting boolean</returns>
            public bool RequestBool(string value) => dictionary.RequestBool(value);

            /// <summary>
            /// Request a property as a double
            /// </summary>
            /// <param name="value">The name of the property</param>
            /// <returns>The resulting double</returns>
            public double RequestDouble(string value) => dictionary.RequestDouble(value);

            /// <summary>
            /// Request a property as a float
            /// </summary>
            /// <param name="value">The name of the property</param>
            /// <returns>The resulting float</returns>
            public float RequestFloat(string value) => dictionary.RequestFloat(value);

            /// <summary>
            /// Request a property as an integer
            /// </summary>
            /// <param name="value">The name of the property</param>
            /// <returns>The resulting integer</returns>
            public int RequestInt(string value) => dictionary.RequestInt(value);

            /// <summary>
            /// Request a property as a blabbeur object
            /// </summary>
            /// <param name="value">The name of the property</param>
            /// <returns>The resulting blabbeur object</returns>
            public BlabbeurObject RequestObject(string value) => dictionary.RequestObject(value);

            /// <summary>
            /// Request a property as a string
            /// </summary>
            /// <param name="value">The name of the property</param>
            /// <returns>The resulting string</returns>
            public string RequestString(string value) => dictionary.RequestString(value);

            #endregion Property Dictionary and Object Interface
        }
    }
}