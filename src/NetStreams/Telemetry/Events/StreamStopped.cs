using NetStreams.Telemetry;

namespace NetStreams.Telemetry.Events
{
    public class StreamStopped : NetStreamTelemetryEvent
    {
        public StreamStopped(string streamProcessorName) : base(streamProcessorName)
        {
        }

        public StreamStopped()
        {

        }
    }
}