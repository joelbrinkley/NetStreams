using Confluent.Kafka;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;
using NetStreams.Internal;
using NetStreams.Internal.Exceptions;
using System.Reflection;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace NetStreams.Serialization
{
    public class HeaderSerializationStrategy<TType> : ISerializer<TType>, IDeserializer<TType>
    {
        public TType Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            try
            {
                var typeHeader = context.Headers.FirstOrDefault(c => c.Key == NetStreamConstants.HEADER_TYPE);

                var str = Encoding.UTF8.GetString(data.ToArray());

                if (isNull) return default(TType);

                if (typeHeader == null) return JsonConvert.DeserializeObject<TType>(str);

                var typeString = Encoding.UTF8.GetString(typeHeader.GetValueBytes());

                var type = Type.GetType(typeString, AssemblyResolver, null);

                return (TType) JsonConvert.DeserializeObject(str, type);
            }
            catch (InvalidCastException ice)
            {
                throw new MalformedMessageException("Encountered a malformed message.", ice);
            }
        }

        public byte[] Serialize(TType data, SerializationContext context)
        {
            var json = JsonConvert.SerializeObject(data);
            return Encoding.UTF8.GetBytes(json);
        }

        private static ConcurrentDictionary<AssemblyName, Assembly> _assemblyCache = new ConcurrentDictionary<AssemblyName, Assembly>();

        private static Assembly AssemblyResolver(AssemblyName assemblyName)
        {
            assemblyName.Version = null;

            var fetched = _assemblyCache.TryGetValue(assemblyName, out var assembly);

            if (!fetched)
            {
                assembly = Assembly.Load(assemblyName);
                _assemblyCache.TryAdd(assemblyName, assembly);
            }

            return assembly;
        }
    }

}
