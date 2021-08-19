using NetStreams.Telemetry;
using System.Threading;
using System.Threading.Tasks;

namespace NetStreams.Internal
{
    internal class NoOpTelemetryClient : INetStreamTelemetryClient
    {
        public Task SendAsync(NetStreamsTelemetryEvent telemetryEvent, CancellationToken token)
        {
            return Task.CompletedTask;
        }
    }
}