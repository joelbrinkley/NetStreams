using System;
using System.Threading;
using System.Threading.Tasks;
using NetStreams.Configuration;
using NetStreams.Internal;

namespace NetStreams
{
    public interface INetStream<TKey, TMessage> : INetStream
    {
        INetStream<TKey, TMessage> Filter(Func<IConsumeContext<TKey, TMessage>, bool> filterPredicate);
        ConsumeProcessor<TKey, TMessage> Transform(Func<IConsumeContext<TKey, TMessage>, object> handleConsumeContext);
        ConsumeProcessor<TKey, TMessage> TransformAsync(Func<IConsumeContext<TKey, TMessage>, Task<object>> handleConsumeContext);
        INetStream<TKey, TMessage> Handle(Action<IConsumeContext<TKey, TMessage>> handleConsumeContext);
        INetStream<TKey, TMessage> HandleAsync(Func<IConsumeContext<TKey, TMessage>, Task> handleTask);
        INetStream<TKey, TMessage> OnError(Action<Exception> onError);
    }

    public interface INetStream : IDisposable
    { 
        INetStreamConfigurationContext Configuration { get; }
        Task StartAsync(CancellationToken token);
    }
}
