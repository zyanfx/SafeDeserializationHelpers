namespace SafeDeserializationHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Blacklist-based delegate validator.
    /// </summary>
    public class DelegateValidator
    {
        /// <summary>
        /// The default blacklist of the namespaces.
        /// </summary>
        private static readonly string[] DefaultBlacklistedNamespaces = new[]
        {
            "System.IO",
            "System.Diagnostics",
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateValidator"/> class.
        /// </summary>
        /// <param name="blacklistedNamespaces">Namespace blacklist.</param>
        public DelegateValidator(params string[] blacklistedNamespaces)
        {
            if (blacklistedNamespaces == null || blacklistedNamespaces.Length == 0)
            {
                blacklistedNamespaces = DefaultBlacklistedNamespaces;
            }

            BlacklistedNamespaces = new HashSet<string>(blacklistedNamespaces, StringComparer.OrdinalIgnoreCase);
        }

        private HashSet<string> BlacklistedNamespaces { get; }

        /// <summary>
        /// Validates the given delegates.
        /// Throws exceptions for methods defined in the blacklisted namespaces.
        /// </summary>
        /// <param name="del">The delegate to validate.</param>
        public void ValidateDelegate(Delegate del)
        {
            if (del == null)
            {
                return;
            }

            foreach (var d in del.GetInvocationList())
            {
                if (d == null)
                {
                    continue;
                }

                var type = d.Method.DeclaringType;
                if (BlacklistedNamespaces.Contains(type.Namespace))
                {
                    var msg = $"Deserializing delegates for {type.FullName} may be unsafe.";
                    throw new UnsafeDeserializationException(msg);
                }
            }
        }
    }
}
