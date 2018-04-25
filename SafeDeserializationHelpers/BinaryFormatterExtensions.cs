namespace SafeDeserializationHelpers
{
    using System.Data;
    using System.Runtime.Serialization;
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
            // safe type binder prevents delegate deserialization attacks
            var binder = new SafeSerializationBinder(fmt.Binder);
            fmt.Binder = binder;

            // DataSet surrogate validates binary-serialized datasets
            var ss = new SurrogateSelector();
            ss.AddSurrogate(typeof(DataSet), new StreamingContext(StreamingContextStates.All), new DataSetSurrogate());
            fmt.SurrogateSelector = ss;

            // TODO: do we need to chain surrogate selectors?
            return fmt;
        }
    }
}
