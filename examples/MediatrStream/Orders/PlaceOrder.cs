using MediatR;


namespace MediatrStream.Orders
{
    public class PlaceOrder : OrderCommand, IRequest<OrderPlaced>
    {
        public string CustomerId { get; set; }
        public string OrderDescription { get; set; }

        public override string Key => CustomerId;
    }
}

