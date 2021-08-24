using NetStreams.Telemetry;

namespace NetStreams.Internal
{
    internal class OffsetResetted : NetStreamsTelemetryEvent
    {
        public OffsetResetted(string streamProcessorName) : base(streamProcessorName)
        {

        }
    }
}