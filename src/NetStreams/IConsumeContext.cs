namespace NetStreams
{
    public interface IConsumeContext<TKey, TMessage>
    {
        TKey Key { get; }
        TMessage Message { get; }
    }
}
