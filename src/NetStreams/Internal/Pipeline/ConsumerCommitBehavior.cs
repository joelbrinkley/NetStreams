﻿using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace NetStreams.Internal.Pipeline
{
    internal class ConsumerCommitBehavior <TKey, TMessage> : PipelineStep<TKey, TMessage>
    {
        readonly IConsumer<TKey, TMessage> _consumer;

        public ConsumerCommitBehavior(IConsumer<TKey, TMessage> consumer)
        {
            _consumer = consumer;
        }

        public override async Task<NetStreamResult> Handle(IConsumeContext<TKey, TMessage> consumeContext, CancellationToken token, NetStreamResult result)
        {
            if (Next != null) return await Next.Handle(consumeContext, token, result);

           _consumer.Commit();

           return result;
        }
    }
}
