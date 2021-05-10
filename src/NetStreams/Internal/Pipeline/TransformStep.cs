using System;
using System.Threading;
using System.Threading.Tasks;

namespace NetStreams.Internal.Pipeline
{
    internal class AsyncConsumeTransformer<TKey, TMessage> : PipelineStep<TKey, TMessage>
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

        public override async Task<NetStreamResult> Handle(IConsumeContext<TKey, TMessage> consumeContext, NetStreamResult result, CancellationToken token)
        {
            var response = await _handle(consumeContext);

            result = new NetStreamResult(response);

            return await this.Next.Handle(consumeContext, result, token);
        }
    }

    internal class ConsumeTransformer<TKey, TMessage> : PipelineStep<TKey, TMessage>
    {
        readonly Func<IConsumeContext<TKey, TMessage>, object> _handle;

        public ConsumeTransformer(Func<IConsumeContext<TKey, TMessage>, object> handle)
        {
            _handle = handle;
        }
        public override async Task<NetStreamResult> Handle(IConsumeContext<TKey, TMessage> consumeContext, NetStreamResult result, CancellationToken token)
        {
            var response = _handle(consumeContext);

           if(Next != null) return await Next.Handle(consumeContext, new NetStreamResult(response), token);

           return new NetStreamResult(response);
        }
    }
}