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
        public class MyMessage : IMessage
        {
            public int Value { get; set; }
            public string Key => NullKey.Value;

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

            var startTask = builder.Stream<MyMessage>(sourceTopic)
                                   .Filter(context => context.Message.Value % 3 == 0)
                                   .Handle(context => Console.WriteLine($"Handling message value={context.Message.Value}"))
                                   .StartAsync(new CancellationToken());


            IProducer<string, MyMessage> producer = new ProducerBuilder<string, MyMessage>(new ProducerConfig() { BootstrapServers = "localhost:9092" })
                                                    .SetValueSerializer(new JsonSer<MyMessage>())
                                                    .Build();

            for (int i = 0; i < 100; i++)
            {
                var message = new MyMessage() { Value = i };
                producer.ProduceAsync(sourceTopic, new Message<string, MyMessage> { Key = message.Key, Value = message });
            }

            Task.WaitAll(startTask);
        }
    }
}
