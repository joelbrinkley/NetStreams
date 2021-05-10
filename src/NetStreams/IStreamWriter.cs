using System.Threading.Tasks;

namespace NetStreams
{
    public interface IStreamWriter<TKey, TMessage> : IStreamWriter
    {
        Task WriteAsync(TMessage context);
    }
    public interface IStreamWriter
    {
        Task WriteAsync(object context);
    }
}
