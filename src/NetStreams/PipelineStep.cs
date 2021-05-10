﻿using System.Threading;
using System.Threading.Tasks;

namespace NetStreams
{
    public abstract class PipelineStep<TKey, TMessage>
    {
        public PipelineStep<TKey, TMessage> Next { get; set; }

        public virtual async Task<NetStreamResult> Handle(IConsumeContext<TKey, TMessage> consumeContext, NetStreamResult result, CancellationToken token)
        {
            if (this.Next != null)
            {
                 return await this.Next.Handle(consumeContext, result, token);
            }

            return new NetStreamResult(null);
        }
    }
}
