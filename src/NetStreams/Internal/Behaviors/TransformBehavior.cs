using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace NetStreams.Internal.Behaviors
{
    internal class AsyncConsumeTransformer<TKey, TMessage> : ConsumeBehavior<TKey, TMessage>
    {
        readonly Func<IConsumeContext<TKey, TMessage>, Task<object>> _handle;
        readonly INetStream<TKey, TMessage> _stream;
        readonly IStreamWriter _writer;

        public INetStream Stream => _stream;

        public AsyncConsumeTransformer(
            Func<IConsumeContext<TKey, TMessage>, Task<object>> handle,
            INetStream<TKey, TMessage> netStream,
            IStreamWriter streamWriter)
        {
            _handle = handle;
            _stream = netStream;
            _writer = streamWriter;
        }

        public override async Task Handle(IConsumeContext<TKey, TMessage> consumeContext, CancellationToken token)
        {
            var response = await _handle(consumeContext);

            if (response != null && _writer != null)
            {
                await _writer.WriteAsync(response);
            }
        }
    }

    internal class ConsumeTransformer<TKey, TMessage> : ConsumeBehavior<TKey, TMessage>
    {
        readonly Func<IConsumeContext<TKey, TMessage>, object> _handle;
        readonly IStreamWriter _writer;
        public INetStream Stream { get; }

        public ConsumeTransformer(
            Func<IConsumeContext<TKey, TMessage>, object> handle, 
            INetStream<TKey, TMessage> netStream,
            IStreamWriter writer)
        {
            _handle = handle;
            _writer = writer;
            Stream = netStream;
        }
        public override async Task Handle(IConsumeContext<TKey, TMessage> consumeContext, CancellationToken token)
        {
            var response = _handle(consumeContext);

            if (response != null && _writer != null)
            {
                await _writer.WriteAsync(response);
            }
        }
    }
}