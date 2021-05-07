using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Threading.Tasks;

namespace NetStreams
{
    public abstract class ConsumeBehavior<TKey, TMessage>
    {
        public ConsumeBehavior<TKey, TMessage> Next { get; set; }
        
        public virtual Task<TransformResult> Handle(IConsumeContext<TKey, TMessage> consumeContext, CancellationToken token)
        {
            if (this.Next != null)
            {
                return this.Next.Handle(consumeContext, token);
            }
            else
            {
                return null;
            }
        }
    }
}
