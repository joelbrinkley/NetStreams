using System;
using System.Threading.Tasks;
using NetStreams.Configuration;

namespace NetStreams
{
    public interface INetStreamBuilder<TKey, TMessage> : INetStreamBuilder
    {
        INetStreamBuilder<TKey, TMessage> Name(string name);
        INetStreamBuilder<TKey, TMessage> Stream(string sourceTopic);
        INetStreamBuilder<TKey, TMessage> Filter(Func<IConsumeContext<TKey, TMessage>, bool> filterPredicate);
        INetStreamBuilder<TKey, TMessage> Transform(Func<IConsumeContext<TKey, TMessage>, object> handleConsumeContext);
        INetStreamBuilder<TKey, TMessage> TransformAsync(Func<IConsumeContext<TKey, TMessage>, Task<object>> handleConsumeContext);
        INetStreamBuilder<TKey, TMessage> Handle(Action<IConsumeContext<TKey, TMessage>> handleConsumeContext);
        INetStreamBuilder<TKey, TMessage> HandleAsync(Func<IConsumeContext<TKey, TMessage>, Task> handleTask);
        INetStreamBuilder<TKey, TMessage> ToTopic<TResponseKey, TResponseMessage>(string topic, Func<TResponseMessage, TResponseKey> resolveKey = null);
        INetStreamBuilder<TKey, TMessage> OnError(Action<Exception> onError);
    }

    public interface INetStreamBuilder
    {
        INetStreamConfigurationContext Configuration { get; }
        INetStream Build();
    }
}