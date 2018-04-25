namespace SafeDeserializationHelpers
{
    using System;
    using System.Runtime.Serialization;

    /// <inheritdoc cref="SerializationBinder" />
    public class SafeSerializationBinder : SerializationBinder
    {
        /// <summary>
        /// Core library assembly name.
        /// </summary>
        public const string CoreLibraryAssemblyName = "mscorlib";

        /// <summary>
        /// System.DelegateSerializationHolder type name.
        /// </summary>
        public const string DelegateSerializationHolderTypeName = "System.DelegateSerializationHolder";

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeSerializationBinder"/> class.
        /// </summary>
        /// <param name="nextBinder">Next serialization binder in chain.</param>
        public SafeSerializationBinder(SerializationBinder nextBinder = null)
        {
            NextBinder = nextBinder;
        }

        private SerializationBinder NextBinder { get; }

        /// <inheritdoc cref="SerializationBinder" />
        public override Type BindToType(string assemblyName, string typeName)
        {
            // prevent delegate serialization attack
            if (typeName == DelegateSerializationHolderTypeName &&
                assemblyName.StartsWith(CoreLibraryAssemblyName, StringComparison.InvariantCultureIgnoreCase))
            {
                return typeof(CustomDelegateSerializationHolder);
            }

            // suppress known blacklisted types
            TypeNameValidator.Default.ValidateTypeName(assemblyName, typeName);

            // chain to the original type binder if exists
            return NextBinder?.BindToType(assemblyName, typeName);
        }
    }
}
