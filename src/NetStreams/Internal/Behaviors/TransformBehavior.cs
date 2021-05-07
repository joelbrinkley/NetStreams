using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace NetStreams.Internal.Behaviors
{
    internal class AsyncConsumeTransformer<TKey, TMessage> : ConsumeBehavior<TKey, TMessage>
    {
        readonly Func<IConsumeContext<TKey, TMessage>, Task<object>> _handle;
        readonly IStreamWriter _writer;
        
        public AsyncConsumeTransformer(
            Func<IConsumeContext<TKey, TMessage>, Task<object>> handle,
            IStreamWriter streamWriter)
        {
            _handle = handle;
            _writer = streamWriter;
        }

        public override async Task Handle(IConsumeContext<TKey, TMessage> consumeContext, CancellationToken token)
        {
            var response = await _handle(consumeContext);

            consumeContext.Result = response;

            await this.Next.Handle(consumeContext, token);
        }
    }

    internal class ConsumeTransformer<TKey, TMessage> : ConsumeBehavior<TKey, TMessage>
    {
        readonly Func<IConsumeContext<TKey, TMessage>, object> _handle;

        public ConsumeTransformer(Func<IConsumeContext<TKey, TMessage>, object> handle)
        {
            _handle = handle;
        }
        public override async Task Handle(IConsumeContext<TKey, TMessage> consumeContext, CancellationToken token)
        {
            var response = _handle(consumeContext);

            consumeContext.Result = response;

           if(Next != null) await Next.Handle(consumeContext, token);
        }
    }
}