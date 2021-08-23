using System;
using System.Threading;
using System.Threading.Tasks;

namespace NetStreams.Internal.Pipeline
{
    internal class Filter<TKey, TMessage> : PipelineStep<TKey, TMessage>
    {
        readonly Func<IConsumeContext<TKey, TMessage>, bool> _filter;

        public Filter(Func<IConsumeContext<TKey, TMessage>, bool> filter)
        {
            _filter = filter;
        }
        public override async Task<NetStreamResult> ExecuteAsync(IConsumeContext<TKey, TMessage> consumeContext, CancellationToken token, NetStreamResult result = null)
        {
            if (Next == null) return new NetStreamResult(null);
            
            if (_filter(consumeContext))
            {
                return await Next.ExecuteAsync(consumeContext, token, new NetStreamResult(consumeContext.Message));
            }

            return new NetStreamResult(null);
        }
    }
}
