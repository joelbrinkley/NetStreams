using System.Threading;
using System.Threading.Tasks;

namespace NetStreams.Internal.Pipeline
{
    internal class WriteOutputToKafkaBehavior<TKey, TMessage> : PipelineStep<TKey, TMessage>
    {
        readonly IStreamWriter _writer;

        public WriteOutputToKafkaBehavior(IStreamWriter writer)
        {
            _writer = writer;
        }

        public override async Task<NetStreamResult> Handle(IConsumeContext<TKey, TMessage> consumeContext, CancellationToken token, NetStreamResult result )
        {
            if (result != null && result.HasValue && _writer != null)
            {
                await _writer.WriteAsync(result.Message);
            }

            return result;
        }
    }
}
