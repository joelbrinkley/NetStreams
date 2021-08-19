using NetStreams.Telemetry;

namespace NetStreams.Internal
{
    internal class StreamStopped : NetStreamsTelemetryEvent
    {
        public StreamStopped(string streamProcessorName) : base(streamProcessorName)
        {
        }
    }
}