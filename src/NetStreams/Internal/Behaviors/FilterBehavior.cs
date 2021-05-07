using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetStreams.Internal.Behaviors
{
    internal class FilterBehavior<TKey, TMessage> : ConsumeBehavior<TKey, TMessage>
    {
        Func<IConsumeContext<TKey, TMessage>, bool> _filter;

        public FilterBehavior(Func<IConsumeContext<TKey, TMessage>, bool> filter)
        {
            _filter = filter;
        }
        public override async Task Handle(IConsumeContext<TKey, TMessage> consumeContext, CancellationToken token)
        {
            if (Next == null) return;

            if (_filter(consumeContext))
            {
                await Next.Handle(consumeContext, token);
            }
        }
    }
}
