using Blabbeur.Objects;
using System;
using UnityEngine;

namespace Blabbeur
{
    namespace Unity
    {
        /// <summary>
        /// A structure that allows a property to be edited within unity's inspector
        /// </summary>
        [Serializable]
        public struct UnityProperty
        {
            /// <summary>
            /// The name of the property
            /// </summary>
            [SerializeField] public string name;

            /// <summary>
            /// The value of the property
            /// </summary>
            [SerializeField] public string value;

            /// <summary>
            /// The type of the property
            /// </summary>
            [SerializeField] public ValueType type;

            /// <summary>
            /// Output the contents of the property as a string
            /// </summary>
            /// <returns>The output</returns>
            public override string ToString() => string.Format("{0}: {1} [{2}]", name, value, type);

            /// <summary>
            /// Convert the value in this object into an actual property
            /// </summary>
            /// <returns>The UnityProperty as a Property</returns>
            public Property ToProperty()
            {
                switch (type)
                {
                    case ValueType.BOOL:
                        return new Property(name, bool.Parse(value), type);

                    case ValueType.DOUBLE:
                        return new Property(name, double.Parse(value), type);

                    case ValueType.FLOAT:
                        return new Property(name, float.Parse(value), type);

                    case ValueType.INTEGER:
                        return new Property(name, int.Parse(value), type);

                    case ValueType.STRING:
                        return new Property(name, value, type);

                    default:
                        return new Property(name, value, ValueType.INVALID);
                }
            }
        }
    }
}