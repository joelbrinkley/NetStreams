using NetStreams.Configuration;

namespace NetStreams
{
    public interface IProducerFactory
    {
        IMessageProducer<TKey, TMessage> Create<TKey, TMessage>(INetStreamConfigurationContext config);
    }
}
