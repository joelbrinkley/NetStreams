using NetStreams.Telemetry;

namespace NetStreams.Internal
{
    internal class ResetOffset : NetStreamsTelemetryEvent
    {
        public ResetOffset(string streamProcessorName) : base(streamProcessorName)
        {

        }
    }
}