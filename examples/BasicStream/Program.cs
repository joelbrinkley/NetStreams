using Confluent.Kafka;
using NetStreams;
using NetStreams.Serialization;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BasicStream
{
    class Program
    {
        public class MyMessage
        {
            public int Value { get; set; }

        }
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
                });

            var startTask = builder.Stream<Null, MyMessage>(sourceTopic)
                                   .Filter(context => context.Message.Value % 3 == 0)
                                   .Handle(context => Console.WriteLine($"Handling message value={context.Message.Value}"))
                                   .StartAsync(new CancellationToken());


            IProducer<Null, MyMessage> producer = new ProducerBuilder<Null, MyMessage>(new ProducerConfig() { BootstrapServers = "localhost:9092" })
                                                    .SetValueSerializer(new NetStreamSerializer<MyMessage>())
                                                    .Build();

            for (int i = 0; i < 100; i++)
            {
                var message = new MyMessage() { Value = i };
                producer.ProduceAsync(sourceTopic, new Message<Null, MyMessage> { Value = message });
            }

            Task.WaitAll(startTask);
        }
    }
}
