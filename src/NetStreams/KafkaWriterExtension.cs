using System;
using Confluent.Kafka;
using NetStreams.Internal;

namespace NetStreams
{
    public static class KafkaWriterExtension
    {
        public static INetStream ToTopic<TResponseKey, TResponse>(
            this ITransform transform, 
            string topic,
            Func<TResponse, TResponseKey> resolveKey = null)
        {
            transform.SetWriter(new KafkaTopicWriter<TResponseKey, TResponse>(topic, new ProducerFactory(), transform, resolveKey));
            return transform.Stream;
        }
    }
}
