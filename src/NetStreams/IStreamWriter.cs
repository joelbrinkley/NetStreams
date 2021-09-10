using System.Threading;
using System.Threading.Tasks;

namespace NetStreams
{
    public interface IStreamWriter<TKey, TMessage> : IStreamWriter
    {
        Task WriteAsync(NetStreamResult<TMessage> result, CancellationToken token);
    }
    public interface IStreamWriter
    {
        Task WriteAsync(NetStreamResult netStreamResult, CancellationToken token);
    }
}
