namespace SafeDeserializationHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Text;

    /// <summary>
    /// Custom replacement for the DelegateSerializationHolder featuring delegate validation.
    /// </summary>
    [Serializable]
    public class CustomDelegateSerializationHolder : ISerializable, IObjectReference
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomDelegateSerializationHolder"/> class.
        /// </summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="context">Streaming context</param>
        protected CustomDelegateSerializationHolder(SerializationInfo info, StreamingContext context)
        {
            Holder = (IObjectReference)Constructor.Invoke(new object[] { info, context });
        }

        private static Type DelegateSerializationHolderType { get; } = Type.GetType(SafeSerializationBinder.DelegateSerializationHolderTypeName);

        private static ConstructorInfo Constructor { get; } = DelegateSerializationHolderType.GetConstructor(
            BindingFlags.Instance | BindingFlags.NonPublic,
            null,
            new[] { typeof(SerializationInfo), typeof(StreamingContext) },
            null);

        private IObjectReference Holder { get; set; }

        /// <inheritdoc cref="ISerializable" />
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc cref="IObjectReference" />
        public object GetRealObject(StreamingContext context)
        {
            var result = Holder.GetRealObject(context);
            if (result is Delegate del)
            {
                DelegateValidator.Default.ValidateDelegate(del);
            }

            return result;
        }
    }
}
