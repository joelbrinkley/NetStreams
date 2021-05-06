using System.Threading.Tasks;

namespace NetStreams
{
    public interface ITransform<TKey, TMessage> :ITransform
    {
        Task<TransformResult> Handle(IConsumeContext<TKey, TMessage> consumeContext);
    }

    public interface ITransform
    {
        INetStream Stream { get; }
        void SetWriter(IStreamWriter writer);
    }
}
