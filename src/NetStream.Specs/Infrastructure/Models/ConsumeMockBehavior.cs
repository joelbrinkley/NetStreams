using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NetStreams.Specs.Infrastructure.Models
{
    public class ConsumeMockBehavior<TKey, TMessage> : ConsumeBehavior<TKey, TMessage>
    {
        public readonly List<TMessage> List;

        public ConsumeMockBehavior(List<TMessage> list)
        {
            List = list;
        }
        public override async Task Handle(IConsumeContext<TKey, TMessage> consumeContext, CancellationToken token)
        {
            List.Add(consumeContext.Message);

            await Next.Handle(consumeContext, token);
        }
    }
}
