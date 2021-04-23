using System;
using System.Threading;
using System.Threading.Tasks;
using NetStreams.Configuration;

namespace NetStreams
{
    public interface INetStream<TKey, TMessage> : IDisposable
    {
        INetStreamConfigurationContext Configuration { get; }
        INetStream<TKey, TMessage> Filter(Func<IConsumeContext<TKey, TMessage>, bool> filterPredicate);
        IHandle<TKey, TMessage, TResponseKey, TResponse> Handle<TResponseKey, TResponse>(Func<IConsumeContext<TKey, TMessage>, TResponse> handleConsumeContext);
        IHandle<TKey, TMessage, TResponseKey, TResponse> HandleAsync<TResponseKey, TResponse>(Func<IConsumeContext<TKey, TMessage>, Task<TResponse>> handleConsumeContext);
        INetStream<TKey, TMessage> Handle(Action<IConsumeContext<TKey, TMessage>> handleConsumeContext);
        INetStream<TKey, TMessage> HandleAsync(Func<IConsumeContext<TKey, TMessage>, Task> handleTask);
        Task StartAsync(CancellationToken token);
    }
}
