using NetStreams.Telemetry;

namespace NetStreams.Internal
{
    internal class MalformedMessageSkipped : NetStreamsTelemetryEvent
    {
        public MalformedMessageSkipped(string streamProcessorName) : base(streamProcessorName)
        {
        }
    }
}