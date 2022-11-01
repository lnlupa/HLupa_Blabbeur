namespace Blabbeur
{
    namespace Objects
    {
        /// <summary>
        /// An interface to a text generation object. A blabbeur object is an object that can be used by blabbeur during the process of creating a line of text
        /// </summary>
        public interface BlabbeurObject
        {
            /// <summary>
            /// All objects must have an identifier
            /// </summary>
            string ID { get; set; }

            /// <summary>
            /// Used to request an integer from the object by name
            /// </summary>
            /// <param name="value">The name of the integer to find</param>
            /// <returns>An integer</returns>
            int RequestInt(string value);

            /// <summary>
            /// Used to request a double from the object by name
            /// </summary>
            /// <param name="value">The name of the double to find</param>
            /// <returns>A double</returns>
            double RequestDouble(string value);

            /// <summary>
            /// Used to request a string from the object by name
            /// </summary>
            /// <param name="value">The name of the string to find</param>
            /// <returns>A string</returns>
            string RequestString(string value);

            /// <summary>
            /// Used to request a float from the object by name
            /// </summary>
            /// <param name="value">The name of the float to find</param>
            /// <returns>A float</returns>
            float RequestFloat(string value);

            /// <summary>
            /// Used to request a blabbeur object from the object by name, BlabbeurObjects are allowed to recursively reference each other.
            /// </summary>
            /// <param name="value">The name of the BlabbeurObjects to find</param>
            /// <returns>A BlabbeurObjects</returns>
            BlabbeurObject RequestObject(string value);

            /// <summary>
            /// Used to request a boolean from the object by name
            /// </summary>
            /// <param name="value">The name of the boolean to find</param>
            /// <returns>A boolean</returns>
            bool RequestBool(string value);
        }
    }
}