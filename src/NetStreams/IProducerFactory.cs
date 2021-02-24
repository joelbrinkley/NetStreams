using NetStreams.Configuration;

namespace NetStreams
{
    public interface IProducerFactory
    {
        IMessageProducer<TMessage> Create<TMessage>(INetStreamConfigurationContext config) where TMessage: IMessage;
    }
}
