using NetStreams.Telemetry;

namespace NetStreams.Telemetry.Events
{
    public class OffsetResetted : NetStreamTelemetryEvent
    {
        public long Offset { get; set; }

        public OffsetResetted(string streamProcessorName, long offset) : base(streamProcessorName)
        {
            Offset = offset;
        }
    }
}