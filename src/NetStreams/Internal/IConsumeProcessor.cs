using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetStreams.Internal
{
    public interface IConsumeProcessor<TKey, TMessage>
    {
        void AddBehavior(ConsumeBehavior<TKey, TMessage> behavior);
        Task<TransformResult> ProcessAsync(IConsumeContext<TKey, TMessage> context, CancellationToken token);
    }

    public class ConsumeProcessor<TKey, TMessage> : IConsumeProcessor<TKey, TMessage>
    {
        private ConsumeBehavior<TKey, TMessage> _behavior;

        public void AddBehavior(ConsumeBehavior<TKey, TMessage> behavior)
        {
            if (_behavior == null)
            {
                _behavior = behavior;
            }
            else
            {
                _behavior.Next = behavior;
            }
        }

        public async Task<TransformResult> ProcessAsync(IConsumeContext<TKey, TMessage> context, CancellationToken token)
        {
           return await _behavior.Handle(context, token);
        }
    }
}
