namespace SafeDeserializationHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Validates the type names before loading them for deserialization.
    /// </summary>
    public class TypeNameValidator : ITypeNameValidator
    {
        /// <summary>
        /// The default blacklist of the types.
        /// </summary>
        private static readonly string[] DefaultBlacklistedTypes = new[]
        {
            "System.Management.Automation.PSObject, System.Management.Automation, Version=3.0.0.0",
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeNameValidator"/> class.
        /// </summary>
        /// <param name="blacklistedTypes">The list of the blacklisted types.</param>
        public TypeNameValidator(params string[] blacklistedTypes)
        {
            if (blacklistedTypes == null || blacklistedTypes.Length == 0)
            {
                blacklistedTypes = DefaultBlacklistedTypes;
            }

            BlacklistedTypes = new HashSet<TypeFullName>(blacklistedTypes.Select(TypeFullName.Parse));
        }

        /// <summary>
        /// Gets or sets the default <see cref="ITypeNameValidator" /> instance.
        /// </summary>
        public static ITypeNameValidator Default { get; set; } = new TypeNameValidator();

        private HashSet<TypeFullName> BlacklistedTypes { get; }

        /// <inheritdoc cref="ITypeNameValidator" />
        public void ValidateTypeName(string assemblyName, string typeName)
        {
            var fullName = TypeFullName.Parse($"{typeName}, {assemblyName}");
            if (BlacklistedTypes.Contains(fullName))
            {
                var msg = $"Deserialization of the {typeName} type is not allowed.";
                throw new UnsafeDeserializationException(msg);
            }
        }
    }
}
