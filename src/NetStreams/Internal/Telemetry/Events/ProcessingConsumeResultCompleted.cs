using NetStreams.Telemetry;

namespace NetStreams.Internal
{
    internal class ProcessingConsumeResultCompleted<TKey, TMessage> : NetStreamsTelemetryEvent
    {
        public ProcessingConsumeResultCompleted(
            string streamProcessorName, 
            IConsumeContext<TKey, TMessage> consumeContext) 
            : base(streamProcessorName)
        {
        }
    }
}