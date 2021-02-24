﻿using Confluent.Kafka;
using NetStreams.Internal.Exceptions;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;

namespace NetStreams.Serialization
{
    public class JsonSer<TType> : ISerializer<TType>, IDeserializer<TType>
    {
        public TType Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            var typeHeader = context.Headers.FirstOrDefault(c => c.Key == "Type");

            if (isNull || typeHeader == null) return default(TType);

            var typeString = Encoding.UTF8.GetString(typeHeader.GetValueBytes());

            var str = Encoding.UTF8.GetString(data.ToArray());

            var type = Type.GetType(typeString);

            if (type == null) throw new DeserializationException($"Unable to deserialize type {type} found in header.");

            return (TType)JsonConvert.DeserializeObject(str, type);
        }

        public byte[] Serialize(TType data, SerializationContext context)
        {
            var json = JsonConvert.SerializeObject(data);
            return Encoding.UTF8.GetBytes(json);
        }
    }
}
