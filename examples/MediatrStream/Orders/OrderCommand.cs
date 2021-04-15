using NetStreams;

namespace MediatrStream.Orders
{
    public abstract class OrderCommand
    {
        public OrderCommand()
        {

        }

        public abstract string Key { get; }
    }
}
