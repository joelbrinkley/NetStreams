using NetStreams.Telemetry;

namespace NetStreams.Internal
{
    internal class MessageProcessingCompleted<TKey, TMessage> : NetStreamsTelemetryEvent
    {
        public MessageProcessingCompleted(
            string streamProcessorName, 
            IConsumeContext<TKey, TMessage> consumeContext) 
            : base(streamProcessorName)
        {
        }
    }
}