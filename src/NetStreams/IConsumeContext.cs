namespace NetStreams
{
    public interface IConsumeContext<IMessage>
    {
        IMessage Message { get; }
    }
}
