﻿using System;
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
        public override async Task<NetStreamResult> Handle(IConsumeContext<TKey, TMessage> consumeContext, NetStreamResult result, CancellationToken token)
        {
            if (Next == null) return new NetStreamResult(null);

            if (_filter(consumeContext))
            {
                return await Next.Handle(consumeContext, result, token);
            }

            return new NetStreamResult(null);
        }
    }
}
