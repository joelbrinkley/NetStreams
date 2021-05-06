using System;
using System.Threading.Tasks;

namespace NetStreams.Internal
{
    internal class AsyncConsumeHandler<TKey, TMessage> : IHandle<TKey, TMessage>
    {
        IStreamWriter _writer;
        Func<IConsumeContext<TKey, TMessage>, Task<object>> _handle;
        INetStream<TKey, TMessage> _stream;

        public INetStream Stream => _stream;

        public AsyncConsumeHandler(Func<IConsumeContext<TKey, TMessage>, Task<object>> handle, INetStream<TKey, TMessage> netStream)
        {
            _handle = handle;
            _stream = netStream;
        }

        public async Task<HandleResult> Handle(IConsumeContext<TKey, TMessage> consumeContext)
        {
            var response = await _handle(consumeContext);
            if (response != null) await _writer.WriteAsync(response);
            return new HandleResult(response);
        }

        public void SetWriter(IStreamWriter writer)
        {
            _writer = writer;
        }
    }

    internal class ConsumeHandler<TKey, TMessage> : IHandle<TKey, TMessage>
    {
        IStreamWriter _writer;
        readonly Func<IConsumeContext<TKey, TMessage>, object> _handle;
        public INetStream Stream { get; }

        public ConsumeHandler(Func<IConsumeContext<TKey, TMessage>, object> handle, INetStream<TKey, TMessage> netStream)
        {
            _handle = handle;
            Stream = netStream;
        }

        public async Task<HandleResult> Handle(IConsumeContext<TKey, TMessage> consumeContext)
        {
            var response = _handle(consumeContext);
            if (response != null) await _writer.WriteAsync(response);
            return new HandleResult(response);
        }

        public void SetWriter(IStreamWriter writer)
        {
            _writer = writer;
        }
    }
}