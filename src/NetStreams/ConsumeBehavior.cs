﻿using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Threading.Tasks;

namespace NetStreams
{
    public abstract class ConsumeBehavior<TKey, TMessage>
    {
        public ConsumeBehavior<TKey, TMessage> Next { get; set; }

        public virtual async Task Handle(IConsumeContext<TKey, TMessage> consumeContext, CancellationToken token)
        {
            if (this.Next != null)
            {
                 await this.Next.Handle(consumeContext, token);
            }
        }
    }
}
