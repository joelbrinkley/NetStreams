using System;
using Confluent.Kafka;
using NetStreams.Internal;

namespace NetStreams
{
    public static class KafkaWriterExtension
    {
        public static INetStream ToTopic<TResponseKey, TResponse>(
            this IConsumeBehavior consumeBehavior, 
            string topic,
            Func<TResponse, TResponseKey> resolveKey = null)
        {
            consumeBehavior.SetWriter(new KafkaTopicWriter<TResponseKey, TResponse>(topic, new ProducerFactory(), consumeBehavior, resolveKey));
            return consumeBehavior.Stream;
        }
    }
}
