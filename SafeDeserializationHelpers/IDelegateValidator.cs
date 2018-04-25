namespace SafeDeserializationHelpers
{
    using System;

    /// <summary>
    /// Interface for validating the deserialized delegates.
    /// </summary>
    public interface IDelegateValidator
    {
        /// <summary>
        /// Validates the given delegate.
        /// Throws exceptions for the unsafe delegates found in the invocation list.
        /// </summary>
        /// <param name="del">The delegate to validate.</param>
        void ValidateDelegate(Delegate del);
    }
}
