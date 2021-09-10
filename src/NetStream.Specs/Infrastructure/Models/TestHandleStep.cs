using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetStreams.Specs.Infrastructure.Models
{
    public class TestHandleStep<TKey, TMessage> : PipelineStep<TKey, TMessage>
    {
        public string Name { get; set; }
        readonly Action<IConsumeContext<TKey, TMessage>> _handle;

        public TestHandleStep(string name, Action<IConsumeContext<TKey, TMessage>> handle)
        {
            Name = name;
            _handle = handle;
        }
        public override async Task<NetStreamResult> ExecuteAsync(IConsumeContext<TKey, TMessage> consumeContext, CancellationToken token, NetStreamResult result)
        {
            _handle(consumeContext);

            if (Next != null) return await Next.ExecuteAsync(consumeContext, token, new NetStreamResult(null));

            return new NetStreamResult(result.Message);
        }
    }
}
