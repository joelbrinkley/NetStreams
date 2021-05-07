using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetStreams.Internal.Behaviors
{
    internal class WriteOutputToKafkaBehavior<TKey, TMessage> : ConsumeBehavior<TKey, TMessage>
    {
        readonly IStreamWriter _writer;

        public WriteOutputToKafkaBehavior(IStreamWriter writer)
        {
            _writer = writer;
        }
        public override async Task Handle(IConsumeContext<TKey, TMessage> consumeContext, CancellationToken token)
        {
            if (consumeContext.Result != null && _writer != null)
            {
                await _writer.WriteAsync(consumeContext.Result);
            }
        }
    }
}
