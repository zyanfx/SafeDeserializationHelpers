namespace Zyan.SafeDeserializationHelpers
{
    /// <summary>
    /// Interface for validating type names before loading types for deserialization.
    /// </summary>
    public interface ITypeNameValidator
    {
        /// <summary>
        /// Validates the given type name before loading.
        /// Throws exceptions for the types not safe for deserialization.
        /// </summary>
        /// <param name="assemblyName">The name of the assembly.</param>
        /// <param name="typeName">The name of the type.</param>
        void ValidateTypeName(string assemblyName, string typeName);
    }
}
