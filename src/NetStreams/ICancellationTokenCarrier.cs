using System.Threading;

namespace NetStreams
{
    public interface ICancellationTokenCarrier
    {
        CancellationToken CancellationToken { get; }
    }
}
