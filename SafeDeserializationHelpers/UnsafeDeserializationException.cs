namespace Zyan.SafeDeserializationHelpers
{
    using System;
    using System.Runtime.Serialization;
    using System.Security;

    /// <summary>
    /// Exception to be thrown when possible deserialization vulnerability is detected.
    /// </summary>
    [Serializable]
    public class UnsafeDeserializationException : SecurityException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnsafeDeserializationException"/> class.
        /// </summary>
        public UnsafeDeserializationException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsafeDeserializationException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        public UnsafeDeserializationException(string message)
            : base(message)
        {
        }

        /// <inheritdoc cref="SecurityException"/>
        protected UnsafeDeserializationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
