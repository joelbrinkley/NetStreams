using System;
using System.Threading;
using System.Threading.Tasks;
using NetStreams.Configuration;

namespace NetStreams
{
    public interface INetStream : IDisposable
    { 
        INetStreamConfigurationContext Configuration { get; }
        Task StartAsync(CancellationToken token); 
    }
}
