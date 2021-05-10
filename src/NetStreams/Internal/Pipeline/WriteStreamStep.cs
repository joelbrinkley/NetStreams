using System.Threading;
using System.Threading.Tasks;

namespace NetStreams.Internal.Pipeline
{
    internal class WriteStreamStep<TKey, TMessage> : PipelineStep<TKey, TMessage>
    {
        readonly IStreamWriter _writer;

        public WriteStreamStep(IStreamWriter writer)
        {
            _writer = writer;
        }

        public override async Task<NetStreamResult> Execute(IConsumeContext<TKey, TMessage> consumeContext, CancellationToken token, NetStreamResult result )
        {
            if (result != null && result.HasValue && _writer != null)
            {
                await _writer.WriteAsync(result.Message);
            }

            return result;
        }
    }
}
