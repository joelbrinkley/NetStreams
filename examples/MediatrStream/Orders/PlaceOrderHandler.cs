using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace MediatrStream.Orders
{
    public class PlaceOrderHandler : IRequestHandler<PlaceOrder, OrderPlaced>
    {
        public Task<OrderPlaced> Handle(PlaceOrder placeOrder, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Placing order of { placeOrder.OrderDescription} for customer={placeOrder.CustomerId}");

            var orderPlaced = new OrderPlaced()
            {
                CustomerId = placeOrder.CustomerId,
                OrderId = Guid.NewGuid().ToString()
            };

            return Task.FromResult(orderPlaced);
        }
    }
}
