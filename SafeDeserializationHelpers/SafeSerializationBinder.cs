namespace SafeDeserializationHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;

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
            if (typeName == DelegateSerializationHolderTypeName &&
                assemblyName.StartsWith(CoreLibraryAssemblyName, StringComparison.InvariantCultureIgnoreCase))
            {
                return typeof(CustomDelegateSerializationHolder);
            }

            return NextBinder?.BindToType(assemblyName, typeName);
        }
    }
}
