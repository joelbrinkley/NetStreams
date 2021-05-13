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
                GetLast().Next = step;
            }
        }

        public async Task ExecuteAsync(IConsumeContext<TKey, TMessage> context, CancellationToken token)
        {
           await _head.Execute(context, token, new NetStreamResult(context.Message));
        }

        public void PrependStep(PipelineStep<TKey, TMessage> step)
        {
            var currentHead = _head;
            _head = step;
            _head.Next = currentHead;
        }

        PipelineStep<TKey, TMessage> GetLast()
        {
            var current = _head;

            while (current.Next != null)
            {
                current = current.Next;
            }

            return current;
        }
    }
}
