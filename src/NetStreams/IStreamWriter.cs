using System;
using System.Threading.Tasks;

namespace NetStreams
{
    public interface IStreamWriter<TKey, TMessage, TResponseKey, TResponseMessage> : IStreamWriter<TResponseKey, TResponseMessage>
    {
        INetStream<TKey, TMessage> To(string topic);
    }

    public interface IStreamWriter<TKey, TMessage> : IStreamWriter
    {
        Task WriteAsync(TMessage context);
    }

    public interface IStreamWriter : IDisposable
    {
        Task WriteAsync(object context);
    }
}
