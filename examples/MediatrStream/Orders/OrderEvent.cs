using MediatR;

namespace MediatrStream.Orders
{
    public abstract class OrderEvent : INotification
    {
        public abstract string Key { get; }
    }
}
