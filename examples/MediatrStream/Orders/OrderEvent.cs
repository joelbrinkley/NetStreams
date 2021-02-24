using MediatR;
using NetStreams;

namespace MediatrStream.Orders
{
    public abstract class OrderEvent : INotification, IMessage
    {
        public abstract string Key { get; }
    }
}
