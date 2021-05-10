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
        public override async Task<NetStreamResult> Handle(IConsumeContext<TKey, TMessage> consumeContext, NetStreamResult result, CancellationToken token)
        {
            var eventName = $"Consumed from {consumeContext.TopicName}, partition {consumeContext.Partition}";
            var operationName = $"Consume {consumeContext.TopicName}";
            var telemetryEvent = new EventTelemetry(eventName);
            telemetryEvent.Properties.Add("Topic", consumeContext.TopicName);
            telemetryEvent.Properties.Add("Offset", consumeContext.Offset.ToString());
            telemetryEvent.Properties.Add("Partition", consumeContext.Partition.ToString());

            NetStreamResult response;

            using (_client.StartOperation<RequestTelemetry>(operationName))
            {
                response  = await base.Handle(consumeContext, result, token);

                _client.TrackEvent(telemetryEvent);

                _client.GetMetric(
                        $"Lag:{consumeContext.ConsumeGroup}:{consumeContext.TopicName}:{consumeContext.Partition}")
                    .TrackValue(consumeContext.Lag);
            }

            return response;
        }
    }
}
