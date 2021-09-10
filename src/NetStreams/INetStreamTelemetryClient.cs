using NetStreams.Telemetry;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetStreams
{
    public interface INetStreamTelemetryClient
    {
        Task SendAsync(NetStreamTelemetryEvent telemetryEvent, CancellationToken token);
    }
}
