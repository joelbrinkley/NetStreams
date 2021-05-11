using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NetStreams.Configuration;
using NetStreams.Internal.Pipeline;

namespace NetStreams
{
    public class NetStreamBuilder2<TKey, TMessage>
    {
        readonly NetStreamConfiguration _configuration = new NetStreamConfiguration();
        public INetStreamConfigurationContext Configuration => _configuration;

        public NetStreamBuilder2(Action<INetStreamConfigurationBuilderContext> setup)
        {
            setup(_configuration);
        }

        public PipelineStep<ConsumeContext, TMessage, TMessage> Stream()
        {
            return new PipelineStep<ConsumeContext, TMessage, TMessage>(_configuration,
                (_context, inbound) => Task.FromResult(new NetStreamResult<TMessage>(inbound)));
        }
    }
}
