using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetStreams
{
    public interface IConsumePipeline<TKey, TMessage>
    {
        void AppendStep(PipelineStep<TKey, TMessage> step);
        Task ExecuteAsync(IConsumeContext<TKey, TMessage> context, CancellationToken token);
        void PrependStep(PipelineStep<TKey, TMessage> step);
    }
}
