using Microsoft.ApplicationInsights;
using NetStreams.Configuration;
using NetStreams.Serialization;
using System;

namespace NetStreams.ApplicationInsights
{
    public static class NetStreamBuilderExtension
    {
        public static void AddApplicationInsightsOperationBehavior<TKey, TMessage>(this INetStreamConfigurationBuilderContext<TKey, TMessage> builder, TelemetryClient client)
        {
            builder.PipelineSteps.Push(new ApplicationInsightsPipelineStep<TKey, TMessage>(client));
        }

        public static void UseApplicationInsightTelemetryClient<TKey, TMessage>(
            this INetStreamConfigurationBuilderContext<TKey, TMessage> builder,
            TelemetryClient telemetryClient,
            IJsonSerializer jsonSerializer = null)
        {
            builder.SendTelemetry(new AppInsightsTelemetryClient(
                telemetryClient, 
                jsonSerializer ?? new JsonSerializer()));
        }
    }
}
