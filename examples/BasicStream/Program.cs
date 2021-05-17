using Confluent.Kafka;
using NetStreams;
using NetStreams.Serialization;
using System;
using System.Threading;
using System.Threading.Tasks;
using NetStreams.Extensions;
using NetStreams.Internal.Extensions;

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

            var builder = new NetStreamBuilder2<Null, MyMessage>(
                cfg =>
                {
                    cfg.BootstrapServers = "localhost:9092";
                    cfg.ConsumerGroup = "BasicStream.Consumer";
                    cfg.AddTopicConfiguration(cfg =>
                    {
                        cfg.Name = sourceTopic;
                    });
                });

            var startTask = builder.Stream()
                                   .Filter((context, message) => message.Value % 3 == 0)
                                   .Handle(async (context, message) => Console.WriteLine($"Handling message value={message.Value}"))
                                   .Build()
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
}
