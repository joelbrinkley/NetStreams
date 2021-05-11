using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using NetStreams;
using NetStreams.Serialization;
using NetStreams.Serilog;
using Serilog;

namespace SerilogExample
{
    class Program
    {
        public class MyMessage
        {
            public int Value { get; set; }
        }

        static void Main(string[] args)
        {
            var sourceTopic = "Logging.Source";

            var logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            var builder = new NetStreamBuilder<Null, MyMessage>(
                cfg =>
                {
                    cfg.BootstrapServers = "localhost:9092";
                    cfg.ConsumerGroup = "Logging.Consumer";
                    cfg.AddTopicConfiguration(cfg =>
                    {
                        cfg.Name = sourceTopic;
                    });

                    cfg.ConfigureLogging(logging =>
                    {
                        logging.UseSerilog(logger);
                    });
                });

            var startTask = builder.Stream(sourceTopic)
                .Filter(context => context.Message.Value % 3 == 0)
                .Handle(context => Console.WriteLine($"Handling message value={context.Message.Value}"))
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
