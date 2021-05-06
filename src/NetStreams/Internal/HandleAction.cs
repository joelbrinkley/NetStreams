using System;
using System.Threading;
using System.Threading.Tasks;

namespace NetStreams.Internal
{
    internal class AsyncConsumeTransformer<TKey, TMessage> : ConsumeBehavior<TKey, TMessage>
    {
        IStreamWriter _writer;
        Func<IConsumeContext<TKey, TMessage>, Task<object>> _handle;
        INetStream<TKey, TMessage> _stream;

        public INetStream Stream => _stream;

        public AsyncConsumeTransformer(Func<IConsumeContext<TKey, TMessage>, Task<object>> handle, INetStream<TKey, TMessage> netStream)
        {
            _handle = handle;
            _stream = netStream;
        }

        public override async Task<TransformResult> Handle(IConsumeContext<TKey, TMessage> consumeContext, CancellationToken token)
        {
            var response = await _handle(consumeContext);
            if (response != null && response.GetType() != typeof(None)) await _writer.WriteAsync(response);
            return new TransformResult(response);
        }

        public void SetWriter(IStreamWriter writer)
        {
            _writer = writer;
        }
    }

    internal class ConsumeTransformer<TKey, TMessage> : ConsumeBehavior<TKey, TMessage>
    {
        IStreamWriter _writer;
        readonly Func<IConsumeContext<TKey, TMessage>, object> _handle;
        public INetStream Stream { get; }

        public ConsumeTransformer(Func<IConsumeContext<TKey, TMessage>, object> handle, INetStream<TKey, TMessage> netStream)
        {
            _handle = handle;
            Stream = netStream;
        }

        public override async Task<TransformResult> Handle(IConsumeContext<TKey, TMessage> consumeContext, CancellationToken token)
        {
            var response = _handle(consumeContext);
            if (response != null) await _writer.WriteAsync(response);
            return new TransformResult(response);
        }

        public void SetWriter(IStreamWriter writer)
        {
            _writer = writer;
        }
    }
}