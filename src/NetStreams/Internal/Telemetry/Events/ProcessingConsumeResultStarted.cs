using NetStreams.Telemetry;

namespace NetStreams.Internal
{
    internal class ProcessingConsumeResultStarted<TKey, TMessage> : NetStreamsTelemetryEvent
    {
        public ProcessingConsumeResultStarted(
            string streamProcessorName, 
            IConsumeContext<TKey, TMessage> consumeContext) 
            : base(streamProcessorName)
        {
        }
    }
}