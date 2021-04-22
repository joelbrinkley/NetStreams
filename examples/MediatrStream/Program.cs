using Confluent.Kafka;
using MediatR;
using MediatrStream.Orders;
using Microsoft.Extensions.DependencyInjection;
using NetStreams;
using NetStreams.Serialization;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatrStream
{
    class Program
    {
        static void Main(string[] args)
        {
            var sourceTopic = "Order.Commands";
            var mediator = BuildMediator();

            var builder = new NetStreamBuilder(
                cfg =>
                {
                    cfg.BootstrapServers = "localhost:9092";
                    cfg.ConsumerGroup = "Orders.Consumer";
                    cfg.AddTopicConfiguration(cfg =>
                    {
                        cfg.Name = "Order.Commands";
                        cfg.Partitions = 2;
                    });
                    cfg.AddTopicConfiguration(cfg =>
                    {
                        cfg.Name = "Order.Events";
                        cfg.Partitions = 2;
                    });
                });

            var startTask =
                builder.Stream<string, OrderCommand>(sourceTopic)
                       .Handle<string, OrderEvent>(context => (OrderEvent)mediator.Send(context.Message).Result)
                       .ToTopic("Order.Events", message => message.Key)
                       .StartAsync(CancellationToken.None);

            var producer = new ProducerBuilder<string, OrderCommand>(
                            new ProducerConfig() { BootstrapServers = "localhost:9092" })
                                .SetValueSerializer(new HeaderSerializationStrategy<OrderCommand>())
                                .Build();

            var messageProducer = new NetStreamProducer<string, OrderCommand>(sourceTopic, producer);

            for (int i = 0; i < 100; i++)
            {
                var message = new PlaceOrder()
                {
                    CustomerId = Guid.NewGuid().ToString(),
                    OrderDescription = $"{(i % 6) + 1} widgets"
                };

                messageProducer.ProduceAsync(message.Key, message).Wait();
            }

            Task.WaitAll(startTask);
        }

        private static IMediator BuildMediator()
        {
            var services = new ServiceCollection();

            services.AddMediatR(typeof(PlaceOrder));

            var provider = services.BuildServiceProvider();

            return provider.GetRequiredService<IMediator>();
        }
    }
}
