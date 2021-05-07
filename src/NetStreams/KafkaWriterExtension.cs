using System;
using Confluent.Kafka;
using NetStreams.Internal;

namespace NetStreams
{
    public static class KafkaWriterExtension
    {
        public static INetStream ToTopic<TResponseKey, TResponse>(
            this INetStream netStream,
            string topic,
            Func<TResponse, TResponseKey> resolveKey = null)
        {
            netStream.SetWriter(new KafkaTopicWriter<TResponseKey, TResponse>(topic, new ProducerFactory(), netStream, resolveKey));
            return netStream;
        }
    }
}
