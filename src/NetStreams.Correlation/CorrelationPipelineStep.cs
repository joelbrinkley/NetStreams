using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NetStreams.Correlation
{
    public static class CorrelationHeader
    {
        public static string Value = "X-NETSTREAM-CORRELATION";
    }
    public class CorrelationPipelineStep<TKey, TMessage> : PipelineStep<TKey, TMessage>
    {
       

        public override async Task<NetStreamResult> Execute(IConsumeContext<TKey, TMessage> consumeContext, CancellationToken token, NetStreamResult result = null)
        {
            var doesValueExist = consumeContext.Headers.Any(header => header.Key == CorrelationHeader.Value);

            if (!doesValueExist)
            {
                result.Headers.Add(new KeyValuePair<string, string>(CorrelationHeader.Value, Guid.NewGuid().ToString()));
            }
            else
            {
                result.Headers.Add(new KeyValuePair<string, string>(CorrelationHeader.Value, consumeContext.Headers.First(header => header.Key == CorrelationHeader.Value).Value));
            }

            return await base.Execute(consumeContext, token, result);
        }
    }
}
