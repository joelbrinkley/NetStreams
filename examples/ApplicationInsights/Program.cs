using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using NetStreams;
using NetStreams.Serialization;
using NetStreams.ApplicationInsights;

namespace ApplicationInsights
{
    class Program
    {
        public class MyMessage
        {
            public int Value { get; set; }

        }
        static void Main(string[] args)
        {
            var sourceTopic = "AppInsightsExample.Source";

            var appsettings = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddUserSecrets<Program>()
                .Build();

            TelemetryConfiguration configuration = TelemetryConfiguration.CreateDefault();
            configuration.InstrumentationKey = appsettings["ApplicationInsights:InstrumentationKey"].ToString();

            var telemetryClient = new TelemetryClient(configuration);

            var startTask = new NetStreamBuilder<Null, MyMessage>(
                cfg =>
                {
                    cfg.BootstrapServers = "localhost:9092";
                    cfg.ConsumerGroup = "AppInsightsExample.Consumer";
                    cfg.AddTopicConfiguration(cfg =>
                    {
                        cfg.Name = sourceTopic;
                    });

                    cfg.UseApplicationInsights(telemetryClient);
                })
                .Stream(sourceTopic)
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