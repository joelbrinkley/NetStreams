using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace NetStreams.ApplicationInsights
{
    public class ApplicationInsightsPipelineStep<TKey, TMessage> : PipelineStep<TKey,TMessage>
    {
        readonly TelemetryClient _client;

        public ApplicationInsightsPipelineStep(TelemetryClient client)
        {
            _client = client;
        }
        public override async Task<NetStreamResult> Execute(IConsumeContext<TKey, TMessage> consumeContext,CancellationToken token, NetStreamResult result = null)
        {
            var eventName = $"Consumed from {consumeContext.TopicName}, partition {consumeContext.Partition}";
            var operationName = $"Consume from {consumeContext.TopicName}, partition {consumeContext.Partition}, offset {consumeContext.Offset}";
            var telemetryEvent = new EventTelemetry(eventName);
            telemetryEvent.Properties.Add("Topic", consumeContext.TopicName);
            telemetryEvent.Properties.Add("Offset", consumeContext.Offset.ToString());
            telemetryEvent.Properties.Add("Partition", consumeContext.Partition.ToString());

            NetStreamResult response;

            using (_client.StartOperation<RequestTelemetry>(operationName))
            {
                _client.GetMetric(
                        $"Lag:{consumeContext.ConsumeGroup}:{consumeContext.TopicName}:{consumeContext.Partition}")
                    .TrackValue(consumeContext.Lag);

                response  = await base.Execute(consumeContext, token, result);

                _client.TrackEvent(telemetryEvent);
            }

            return response;
        }
    }
}
