using System.Threading.Tasks;

namespace NetStreams
{
    public interface IHandle<T> where T : IMessage
    {
        Task Handle(IConsumeContext<T> consumeContext);
        INetStream<T> Stream { get; }
    }
    public interface IHandle<T, TResponse> : IHandle<T>
        where T : IMessage
        where TResponse : IMessage
    {
        void Write(IStreamWriter<TResponse> writer);
    }

}
