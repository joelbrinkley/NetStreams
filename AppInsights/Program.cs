using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using NetStreams;
using NetStreams.Serialization;

namespace AppInsights
{
    class Program
    {
        static void Main(string[] args)
        {
            var sourceTopic = "BasicStream.Source";

            var builder = new NetStreamBuilder(
                cfg =>
                {
                    cfg.BootstrapServers = "localhost:9092";
                    cfg.ConsumerGroup = "BasicStream.Consumer";
                    cfg.AddTopicConfiguration(cfg =>
                    {
                        cfg.Name = sourceTopic;
                    });

                    //cfg.AddBehavior(new ApplicationInsightsTelemetryBehavior());
;                });

            var startTask = builder.Stream<Null, MyMessage>(sourceTopic)
                .Filter(context => context.Message.Value % 3 == 0)
                .Handle(context => Console.WriteLine($"Handling message value={context.Message.Value}"))
                .StartAsync(new CancellationToken());

            IProducer<Null, MyMessage> producer = new ProducerBuilder<Null, MyMessage>(new ProducerConfig() { BootstrapServers = "localhost:9092" })
                .SetValueSerializer(new HeaderSerializationStrategy<MyMessage>())
                .Build();

            for (int i = 0; i < 100; i++)
            {
                var message = new MyMessage() { Value = i };
                producer.ProduceAsync(sourceTopic, new Message<Null, MyMessage> { Value = message });
            }

            Task.WaitAll(startTask);
        }
    }

    public class MyMessage
    {
        public int Value { get; set; }

    }
}
