using System.Threading.Tasks;

namespace NetStreams
{
    public interface IHandle<TKey, TMessage> :IHandle
    {
        Task<HandleResult> Handle(IConsumeContext<TKey, TMessage> consumeContext);
    }

    public interface IHandle
    {
        INetStream Stream { get; }
        void SetWriter(IStreamWriter writer);
    }
}
