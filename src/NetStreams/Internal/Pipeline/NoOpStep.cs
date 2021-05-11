using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetStreams.Internal.Pipeline
{
    internal class NoOpStep<TKey, TMessage> : PipelineStep<TKey, TMessage>
    {
        public override Task<NetStreamResult> Execute(IConsumeContext<TKey, TMessage> consumeContext, CancellationToken token, NetStreamResult result = null)
        {
            return Task.FromResult(result);
        }
    }
}
