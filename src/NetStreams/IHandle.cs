using System.Threading.Tasks;

namespace NetStreams
{
    public interface IHandle<TKey, TMessage>
    {
        Task Handle(IConsumeContext<TKey, TMessage> consumeContext);
        INetStream<TKey, TMessage> Stream { get; }
    }
    public interface IHandle<TKey, TMessage, TResponseKey, TResponse> : IHandle<TKey, TMessage>
    {
        void Write(IStreamWriter<TKey, TMessage, TResponseKey, TResponse> writer);
    }

}
