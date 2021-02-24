using System;
using System.Threading.Tasks;

namespace NetStreams
{
    public interface IStreamWriter<TMessage, TResponse> : IStreamWriter<TResponse>
        where TMessage : IMessage
        where TResponse : IMessage
    {
        INetStream<TMessage> To(string topic);
    }

    public interface IStreamWriter<TMessage> : IDisposable
    {
        Task WriteAsync(TMessage context);
    }
}
