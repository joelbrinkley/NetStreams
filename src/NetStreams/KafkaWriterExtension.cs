using System;
using Confluent.Kafka;
using NetStreams.Internal;

namespace NetStreams
{
    public static class KafkaWriterExtension
    {
        public static INetStream<TKey, TMessage> ToTopic<TKey, TMessage, TResponseKey, TResponse>(
            this IHandle<TKey, TMessage, TResponseKey, TResponse> handle, 
            string topic,
            Func<TResponse, TResponseKey> resolveKey = null)
        {
            handle.Write(new KafkaTopicWriter<TKey, TMessage, TResponseKey, TResponse>(topic, new ProducerFactory(), handle, resolveKey));
            return handle.Stream;
        }
    }
}
