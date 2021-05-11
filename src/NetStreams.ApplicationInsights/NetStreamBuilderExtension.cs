using Microsoft.ApplicationInsights;
using NetStreams.Configuration;

namespace NetStreams.ApplicationInsights
{
    public static class NetStreamBuilderExtension
    {
        public static void UseApplicationInsights<TKey, TMessage>(this INetStreamConfigurationBuilderContext<TKey, TMessage> builder, TelemetryClient client)
        {
            builder.PipelineSteps.Push(new ApplicationInsightsPipelineStep<TKey, TMessage>(client));
        }
    }
}
