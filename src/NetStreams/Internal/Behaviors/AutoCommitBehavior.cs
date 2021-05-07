using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace NetStreams.Internal.Behaviors
{
    public class AutoCommitBehavior<TKey, TMessage> : ConsumeBehavior<TKey, TMessage>
    {
        readonly IConsumer<TKey, TMessage> _consumer;

        public AutoCommitBehavior(IConsumer<TKey, TMessage> consumer)
        {
            _consumer = consumer;
        }

        public override async Task<TransformResult> Handle(IConsumeContext<TKey, TMessage> consumeContext, CancellationToken token)
        {
            _consumer.Commit();

            return await Next.Handle(consumeContext, token);
        }
    }
}
