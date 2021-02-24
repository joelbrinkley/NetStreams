namespace MediatrStream.Orders
{
    public class OrderPlaced : OrderEvent
    {
        public string OrderId { get; set; }
        public string CustomerId { get; set; }
        public bool IsPreferred { get; set; }

        public override string Key => OrderId;

    }
}
