using NetStreams.Configuration;

namespace NetStreams.Telemetry.Events
{
    public class StreamStarted : NetStreamTelemetryEvent
    {
        public string Source { get; set; }
        public NetStreamConfiguration Configuration { get; set; }

        public StreamStarted(
            string streamProcessorName,
            string source,
            NetStreamConfiguration configuration)
            : base(streamProcessorName)
        {
            Configuration = configuration;
            Source = source;
        }

        public StreamStarted()
        {

        }
    }
}