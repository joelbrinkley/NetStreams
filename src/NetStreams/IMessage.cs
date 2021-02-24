namespace NetStreams
{
    public interface IMessage<TKey>
    {
        TKey Key { get; }
    }

    public interface IMessage : IMessage<string>
    {

    }
}
