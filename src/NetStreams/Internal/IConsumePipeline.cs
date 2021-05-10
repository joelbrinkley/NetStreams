using System.Threading;
using System.Threading.Tasks;

namespace NetStreams.Internal
{
    internal class ConsumePipeline<TKey, TMessage> : IConsumePipeline<TKey, TMessage>
    {
        PipelineStep<TKey, TMessage> _head;

        public void AppendStep(PipelineStep<TKey, TMessage> step)
        {
            if (_head == null)
            {
                _head = step;
            }
            else
            {
                _head.Next = step;
            }
        }

        public async Task ExecuteAsync(IConsumeContext<TKey, TMessage> context, CancellationToken token)
        {
           await _head.Execute(context, token, new NetStreamResult(null));
        }

        public void PrependStep(PipelineStep<TKey, TMessage> step)
        {
            var currentHead = _head;
            _head = step;
            _head.Next = currentHead;
        }
    }
}
