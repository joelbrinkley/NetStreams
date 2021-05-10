using Confluent.Kafka;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;
using NetStreams.Internal;

namespace NetStreams.Serialization
{
    public class HeaderSerializationStrategy<TType> : ISerializer<TType>, IDeserializer<TType>
    {
        public TType Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            var typeHeader = context.Headers.FirstOrDefault(c => c.Key == NetStreamConstants.HEADER_TYPE);

            var str = Encoding.UTF8.GetString(data.ToArray());

            if (isNull) return default(TType);

            if (typeHeader == null) return JsonConvert.DeserializeObject<TType>(str);

            var typeString = Encoding.UTF8.GetString(typeHeader.GetValueBytes());

            var type = Type.GetType(typeString);
            
            return (TType)JsonConvert.DeserializeObject(str, type);
        }

        public byte[] Serialize(TType data, SerializationContext context)
        {
            var json = JsonConvert.SerializeObject(data);
            return Encoding.UTF8.GetBytes(json);
        }
    }
}
