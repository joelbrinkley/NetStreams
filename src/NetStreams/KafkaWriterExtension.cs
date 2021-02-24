using NetStreams.Internal;

namespace NetStreams
{
    public static class KafkaWriterExtension
    {
        public static INetStream<TMessage> ToTopic<TMessage, TResponse>(this IHandle<TMessage, TResponse> handle, string topic)
            where TMessage : IMessage
            where TResponse : IMessage
        {
            handle.Write(new KafkaTopicWriter<TMessage, TResponse>(topic, new ProducerFactory(), handle));
            return handle.Stream;
        }
    }

}
