using System;
using System.Threading;
using System.Threading.Tasks;
using NetStreams.Configuration;

namespace NetStreams
{
    public interface INetStream<T> : IDisposable where T : IMessage
    {
        INetStreamConfigurationContext Configuration { get; }
        INetStream<T> Filter(Func<IConsumeContext<T>, bool> filterPredicate);
        IHandle<T, TResponse> Handle<TResponse>(Func<IConsumeContext<T>, TResponse> handleConsumeContext) where TResponse : IMessage;
        INetStream<T> Handle(Action<IConsumeContext<T>> handleConsumeContext);
        Task StartAsync(CancellationToken token);
    }
}
