using System;
using Confluent.Kafka;
using NetStreams.Internal;

namespace NetStreams
{
    public static class KafkaWriterExtension
    {
        public static INetStream ToTopic<TResponseKey, TResponse>(
            this IHandle handle, 
            string topic,
            Func<TResponse, TResponseKey> resolveKey = null)
        {
            handle.SetWriter(new KafkaTopicWriter<TResponseKey, TResponse>(topic, new ProducerFactory(), handle, resolveKey));
            return handle.Stream;
        }
    }
}
