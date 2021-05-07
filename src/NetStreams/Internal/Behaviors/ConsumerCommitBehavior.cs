using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace NetStreams.Internal.Behaviors
{
    internal class ConsumerCommitBehavior <TKey, TMessage> : ConsumeBehavior<TKey, TMessage>
    {
        readonly IConsumer<TKey, TMessage> _consumer;

        public ConsumerCommitBehavior(IConsumer<TKey, TMessage> consumer)
        {
            _consumer = consumer;
        }

        public override async Task Handle(IConsumeContext<TKey, TMessage> consumeContext, CancellationToken token)
        {
            if (Next != null) await Next.Handle(consumeContext, token);

           _consumer.Commit();
        }
    }
}
