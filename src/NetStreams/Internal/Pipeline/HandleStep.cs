﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetStreams.Internal.Pipeline
{
    public class AsyncHandleStep<TKey, TMessage> : PipelineStep<TKey, TMessage>
    {
        readonly Func<IConsumeContext<TKey, TMessage>, Task> _handle;

        public AsyncHandleStep(Func<IConsumeContext<TKey, TMessage>, Task> handle)
        {
            _handle = handle;
        }

        public override async Task<NetStreamResult> Handle(IConsumeContext<TKey, TMessage> consumeContext, NetStreamResult result, CancellationToken token)
        {
            await _handle(consumeContext);

            return await this.Next.Handle(consumeContext, new NetStreamResult(null), token);
        }
    }

    internal class HandleStep<TKey, TMessage> : PipelineStep<TKey, TMessage>
    {
        readonly Action<IConsumeContext<TKey, TMessage>> _handle;

        public HandleStep(Action<IConsumeContext<TKey, TMessage>> handle)
        {
            _handle = handle;
        }
        public override async Task<NetStreamResult> Handle(IConsumeContext<TKey, TMessage> consumeContext, NetStreamResult result, CancellationToken token)
        {
            _handle(consumeContext);

            if (Next != null) return await Next.Handle(consumeContext, new NetStreamResult(null), token);

            return new NetStreamResult(null);
        }
    }
}
