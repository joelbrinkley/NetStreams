using System;
using System.Threading;
using System.Threading.Tasks;

namespace NetStreams.Internal.Pipeline
{
    internal class AsyncHandleStep<TKey, TMessage> : PipelineStep<TKey, TMessage>
    {
        readonly Func<IConsumeContext<TKey, TMessage>, Task> _handle;

        public AsyncHandleStep(Func<IConsumeContext<TKey, TMessage>, Task> handle)
        {
            _handle = handle;
        }

        public override async Task<NetStreamResult> Execute(IConsumeContext<TKey, TMessage> consumeContext, CancellationToken token, NetStreamResult result)
        {
            await _handle(consumeContext);

            return await this.Next.Execute(consumeContext, token, result);
        }
    }

    internal class HandleStep<TKey, TMessage> : PipelineStep<TKey, TMessage>
    {
        readonly Action<IConsumeContext<TKey, TMessage>> _handle;

        public HandleStep(Action<IConsumeContext<TKey, TMessage>> handle)
        {
            _handle = handle;
        }
        public override async Task<NetStreamResult> Execute(IConsumeContext<TKey, TMessage> consumeContext, CancellationToken token, NetStreamResult result = null)
        {
            _handle(consumeContext);

            return await base.Next.Execute(consumeContext, token, result);
        }
    }
}
