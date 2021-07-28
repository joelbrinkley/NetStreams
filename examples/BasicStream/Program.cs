using Confluent.Kafka;
using NetStreams;
using NetStreams.Authentication;
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

            var builder = new NetStreamBuilder<Null, MyMessage>(
                cfg =>
                {
                    cfg.UseAuthentication(new PlainTextAuthentication());
                    //cfg.UseAuthentication(new SslAuthentication("sslCaCertPath", "sslClientCertPath", "sslClientKeyPath", "sslClientKeyPwd"));
                    //cfg.UseAuthentication(new SaslScram256Authentication("username", "password", "sslCaCertPath"));
                    //cfg.UseAuthentication(new SaslScram512Authentication("username", "password", "sslCaCertPath"));
                    cfg.BootstrapServers = "localhost:9092";
                    cfg.ConsumerGroup = "BasicStream.Consumer";
                    cfg.AddTopicConfiguration(cfg =>
                    {
                        cfg.Name = sourceTopic;
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
