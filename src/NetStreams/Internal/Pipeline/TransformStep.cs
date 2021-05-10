using System;
using System.Threading;
using System.Threading.Tasks;

namespace NetStreams.Internal.Pipeline
{
    internal class AsyncTransformStep<TKey, TMessage> : PipelineStep<TKey, TMessage>
    {
        readonly Func<IConsumeContext<TKey, TMessage>, Task<object>> _handle;

        public AsyncTransformStep(Func<IConsumeContext<TKey, TMessage>, Task<object>> handle)
        {
            _handle = handle;
        }

        public override async Task<NetStreamResult> Execute(IConsumeContext<TKey, TMessage> consumeContext, CancellationToken token, NetStreamResult result)
        {
            var response = await _handle(consumeContext);
            return await this.Next.Execute(consumeContext, token, new NetStreamResult(response));
        }
    }

    internal class TransformStep<TKey, TMessage> : PipelineStep<TKey, TMessage>
    {
        readonly Func<IConsumeContext<TKey, TMessage>, object> _handle;

        public TransformStep(Func<IConsumeContext<TKey, TMessage>, object> handle)
        {
            _handle = handle;
        }
        public override async Task<NetStreamResult> Execute(IConsumeContext<TKey, TMessage> consumeContext, CancellationToken token, NetStreamResult result)
        {
            var response = _handle(consumeContext);

           if(Next != null) return await Next.Execute(consumeContext, token, new NetStreamResult(response));

           return new NetStreamResult(response);
        }
    }
}