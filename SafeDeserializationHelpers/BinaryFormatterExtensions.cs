namespace SafeDeserializationHelpers
{
    using System;
    using System.Runtime.Serialization.Formatters.Binary;

    /// <summary>
    /// Extension methods for the
    /// </summary>
    public static class BinaryFormatterExtensions
    {
        /// <summary>
        /// Makes the <see cref="BinaryFormatter"/> safe.
        /// </summary>
        /// <param name="fmt">The <see cref="BinaryFormatter"/> to guard.</param>
        /// <returns>The safe version of the <see cref="BinaryFormatter"/>.</returns>
        public static BinaryFormatter Safe(this BinaryFormatter fmt)
        {
            if (fmt == null)
            {
                throw new ArgumentNullException(nameof(fmt), "BinaryFormatter is not specified.");
            }

            // safe type binder prevents delegate deserialization attacks
            if (!(fmt.Binder is SafeSerializationBinder))
            {
                fmt.Binder = new SafeSerializationBinder(fmt.Binder);
            }

            // surrogates validate binary-serialized data before deserializing them
            if (!(fmt.SurrogateSelector is SafeSurrogateSelector))
            {
                // create a new surrogate selector and chain to the existing one, if any
                fmt.SurrogateSelector = new SafeSurrogateSelector(fmt.SurrogateSelector);
            }

            return fmt;
        }
    }
}
