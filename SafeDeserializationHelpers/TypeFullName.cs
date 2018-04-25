namespace SafeDeserializationHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Represents the name of a .NET type.
    /// </summary>
    /// <remarks>
    /// AssemblyName doesn't contain Version part.
    /// </remarks>
    public class TypeFullName : IEquatable<TypeFullName>
    {
        /// <summary>
        /// Gets or sets the name of the type.
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// Gets or sets the name of the assembly.
        /// </summary>
        public string AssemblyName { get; set; }

        /// <summary>
        /// Parses the given type name and returns the <see cref="TypeFullName"/> instance.
        /// </summary>
        /// <param name="typeName">Assembly-qualified type name.</param>
        /// <returns>A parsed <see cref="TypeFullName"/> instance.</returns>
        public static TypeFullName Parse(string typeName)
        {
            if (string.IsNullOrEmpty(typeName) || typeName.All(c => char.IsWhiteSpace(c)))
            {
                throw new ArgumentException(nameof(typeName));
            }

            var parts = typeName.Split(',').Select(p => p.Trim()).ToArray();
            if (parts.Length == 0)
            {
                throw new ArgumentException(nameof(typeName));
            }

            var result = new TypeFullName
            {
                TypeName = parts[0],
                AssemblyName = SafeSerializationBinder.CoreLibraryAssemblyName,
            };

            if (parts.Length > 1)
            {
                result.AssemblyName = parts[1];
            }

            return result;
        }

        /// <inheritdoc cref="object" />
        public override int GetHashCode()
        {
            return $"{TypeName}, {AssemblyName}".GetHashCode();
        }

        /// <inheritdoc cref="IEquatable{T}" />
        public bool Equals(TypeFullName other)
        {
            var cmp = StringComparer.OrdinalIgnoreCase;
            return other != null &&
                cmp.Compare(other.TypeName, TypeName) == 0 &&
                cmp.Compare(other.AssemblyName, AssemblyName) == 0;
        }
    }
}
