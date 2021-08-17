

using NetStreams.Configuration;

namespace NetStreams.Correlation
{
    public static class NetStreamBuilderExtension
    {
        public static void EnableMessageCorrelation<TKey, TMessage>(this INetStreamConfigurationBuilderContext<TKey, TMessage> builder)
        {
            builder.PipelineSteps.Push(new CorrelationPipelineStep<TKey, TMessage>());
        }
    }
}
