using NetStreams.Telemetry;

namespace NetStreams.Internal
{
    internal class MessageProcessingStarted<TKey, TMessage> : NetStreamsTelemetryEvent
    {
        public MessageProcessingStarted(
            string streamProcessorName, 
            IConsumeContext<TKey, TMessage> consumeContext) 
            : base(streamProcessorName)
        {
        }
    }
}