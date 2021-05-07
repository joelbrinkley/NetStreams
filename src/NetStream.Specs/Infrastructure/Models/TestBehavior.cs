using System.Threading;
using System.Threading.Tasks;

namespace NetStreams.Specs.Infrastructure.Models
{
    public class TestBehavior<TKey, TMessage> : ConsumeBehavior<TKey, TMessage>
    {
        public override async Task Handle(IConsumeContext<TKey, TMessage> consumeContext, CancellationToken token)
        {
            await Next.Handle(consumeContext, token);
        }
    }
}
