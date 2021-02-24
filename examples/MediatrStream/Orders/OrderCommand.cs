using NetStreams;

namespace MediatrStream.Orders
{
    public abstract class OrderCommand : IMessage
    {
        public OrderCommand()
        {

        }

        public abstract string Key { get; }
    }
}
