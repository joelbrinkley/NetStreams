using NetStreams.Configuration;
using NetStreams.Telemetry;

namespace NetStreams.Internal
{
    internal class StreamStarted : NetStreamsTelemetryEvent
    {
        public string Source { get; }
        public INetStreamConfigurationContext Configuration { get; }

        public StreamStarted(
            string streamProcessorName,
            string source,
            INetStreamConfigurationContext configuration)
            : base(streamProcessorName)
        {
            Configuration = configuration;
            Source = source;
        }
    }
}