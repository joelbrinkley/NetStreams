using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NetStreams.Specs.Infrastructure.Models
{
    public class ConsumeMockBehavior<TKey, TMessage> : PipelineStep<TKey, TMessage>
    {
        public readonly List<TMessage> List;

        public ConsumeMockBehavior(List<TMessage> list)
        {
            List = list;
        }
        public override async Task<NetStreamResult> Handle(IConsumeContext<TKey, TMessage> consumeContext, NetStreamResult result, CancellationToken token)
        {
            List.Add(consumeContext.Message);

            return await Next.Handle(consumeContext, result, token);
        }
    }
}
