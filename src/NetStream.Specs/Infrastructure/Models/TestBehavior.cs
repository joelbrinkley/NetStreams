using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetStreams.Specs.Infrastructure.Models
{
    public class TestBehavior<TKey, TMessage> : ConsumeBehavior<TKey, TMessage>
    {
        public override Task<TransformResult> Handle(IConsumeContext<TKey, TMessage> consumeContext, CancellationToken token)
        {
            return base.Handle(consumeContext, token);
        }
    }
}
