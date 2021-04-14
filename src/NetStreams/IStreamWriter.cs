using System;
using System.Threading.Tasks;

namespace NetStreams
{
    public interface IStreamWriter<TKey, TMessage, TResponseKey, TResponse> : IStreamWriter<TResponseKey, TResponse>
    {
        INetStream<TKey, TMessage> To(string topic);
    }

    public interface IStreamWriter<TKey, TMessage> : IDisposable
    {
        Task WriteAsync(TMessage context);
    }
}
