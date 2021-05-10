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
        public override async Task<NetStreamResult> Execute(IConsumeContext<TKey, TMessage> consumeContext,  CancellationToken token, NetStreamResult result)
        {
            List.Add(consumeContext.Message);

            return await Next.Execute(consumeContext, token, result);
        }
    }
}
