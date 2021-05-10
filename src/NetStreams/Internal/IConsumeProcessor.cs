using System.Threading;
using System.Threading.Tasks;

namespace NetStreams.Internal
{
    public interface IConsumeProcessor<TKey, TMessage>
    {
        void AppendStep(PipelineStep<TKey, TMessage> behavior);
        Task ProcessAsync(IConsumeContext<TKey, TMessage> context, CancellationToken token);
        void PrependStep(PipelineStep<TKey, TMessage> behavior);
    }

    public class ConsumeProcessor<TKey, TMessage> : IConsumeProcessor<TKey, TMessage>
    {
        PipelineStep<TKey, TMessage> _headBehavior;

        public void AppendStep(PipelineStep<TKey, TMessage> behavior)
        {
            if (_headBehavior == null)
            {
                _headBehavior = behavior;
            }
            else
            {
                _headBehavior.Next = behavior;
            }
        }

        public async Task ProcessAsync(IConsumeContext<TKey, TMessage> context, CancellationToken token)
        {
           await _headBehavior.Handle(context, new NetStreamResult(null), token);
        }

        public void PrependStep(PipelineStep<TKey, TMessage> behavior)
        {
            var currentHead = _headBehavior;
            _headBehavior = behavior;
            _headBehavior.Next = currentHead;
        }
    }
}
