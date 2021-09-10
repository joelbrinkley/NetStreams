using System.Threading;
using System.Threading.Tasks;

namespace NetStreams
{
    public abstract class PipelineStep<TKey, TMessage>
    {
        public PipelineStep<TKey, TMessage> Next { get; set; }

        public virtual async Task<NetStreamResult> ExecuteAsync(IConsumeContext<TKey, TMessage> consumeContext, CancellationToken token, NetStreamResult result = null)
        {
            if (this.Next != null)
            {
                 return await this.Next.ExecuteAsync(consumeContext, token, result);
            }

            return new NetStreamResult(null);
        }
    }
}
