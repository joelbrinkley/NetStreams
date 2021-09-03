using NetStreams.Telemetry;

namespace NetStreams.Telemetry.Events
{
    public class MalformedMessageSkipped : NetStreamTelemetryEvent
    {
        public long Offset { get; set; }

        public MalformedMessageSkipped(string streamProcessorName, long offset) : base(streamProcessorName)
        {
            Offset = offset;
        }

        public MalformedMessageSkipped()
        {

        }
    }
}