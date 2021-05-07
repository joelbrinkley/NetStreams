using System;
using System.Threading;
using System.Threading.Tasks;

namespace NetStreams.Internal.Behaviors
{
    internal class AsyncConsumeTransformer<TKey, TMessage> : ConsumeBehavior<TKey, TMessage>
    {
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
            return new TransformResult(response);
        }
    }

    internal class ConsumeTransformer<TKey, TMessage> : ConsumeBehavior<TKey, TMessage>
    {
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
            return new TransformResult(response);
        }
    }
}