using Blabbeur.Objects;
using System;
using UnityEngine;

namespace Blabbeur
{
    namespace Unity
    {
        /// <summary>
        /// This a specific Unity Property for objects, it allows other property behaviours to be drag and dropped.
        /// </summary>
        [Serializable]
        public struct UnityBlabbeurObjectProperty
        {
            /// <summary>
            /// The name of the property
            /// </summary>
            [SerializeField] public string name;

            /// <summary>
            /// The property behaviour we are referring to
            /// </summary>
            [SerializeField] public PropertyBehaviour value;

            /// <summary>
            /// Output the contents of the property as a string
            /// </summary>
            /// <returns>The output</returns>
            public override string ToString() => string.Format("{0} = {1} {2}", name, value, ValueType.OBJECT);

            /// <summary>
            /// Convert the value in this object into an actual property
            /// </summary>
            /// <returns>The UnityProperty as a Property</returns>
            public Property ToProperty() => new Property(name, value, ValueType.OBJECT);
        }
    }
}