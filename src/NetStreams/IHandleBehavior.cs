using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetStreams
{
    public delegate Task<TResponseMessage> RequestHandlerDelegate<TResponseMessage>();

    interface IHandleBehavior<TKey, TMessage, TResponseKey, TResponseMessage>
    {
        Task<TResponseMessage> Handle(ConsumeContext<TKey, TMessage> consumeContext,
            RequestHandlerDelegate<TResponseMessage> next, CancellationToken token);
    }
}
