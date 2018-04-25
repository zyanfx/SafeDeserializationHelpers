namespace SafeDeserializationHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    using System.Text;

    /// <summary>
    /// Custom <see cref="DataSet"/> descendant controlling the deserialization.
    /// </summary>
    [Serializable]
    public sealed class CustomDataSetDeserializer : DataSet, IObjectReference
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomDataSetDeserializer"/> class.
        /// </summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="context">Streaming context</param>
        private CustomDataSetDeserializer(SerializationInfo info, StreamingContext context)
        {
            Info = info;
            Context = context;
        }

        private static ConstructorInfo Constructor { get; } = typeof(DataSet).GetConstructor(
            BindingFlags.Instance | BindingFlags.NonPublic,
            null,
            new[] { typeof(SerializationInfo), typeof(StreamingContext) },
            null);

        private SerializationInfo Info { get; }

        private StreamingContext Context { get; }

        /// <inheritdoc cref="IObjectReference" />
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public object GetRealObject(StreamingContext context)
        {
            Validate(Info);
            return Constructor.Invoke(new object[] { Info, context });
        }

        private void Validate(SerializationInfo info)
        {
            var remotingFormat = SerializationFormat.Xml;
            var schemaSerializationMode = SchemaSerializationMode.IncludeSchema;

            var e = info.GetEnumerator();
            while (e.MoveNext())
            {
                switch (e.Name)
                {
                    case "DataSet.RemotingFormat": // DataSet.RemotingFormat does not exist in V1/V1.1 versions
                        remotingFormat = (SerializationFormat)e.Value;
                        break;

                    case "SchemaSerializationMode.DataSet": // SchemaSerializationMode.DataSet does not exist in V1/V1.1 versions
                        schemaSerializationMode = (SchemaSerializationMode)e.Value;
                        break;
                }
            }

            if (remotingFormat == SerializationFormat.Xml)
            {
                // XML dataset serialization isn't vulnerable
                return;
            }

            throw new UnsafeDeserializationException("Serialized DataSet probably includes malicious data.");
        }
    }
}
