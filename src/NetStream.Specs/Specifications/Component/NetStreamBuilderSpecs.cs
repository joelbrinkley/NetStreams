using System;
using Confluent.Kafka;
using Machine.Specifications;
using NetStreams.Configuration;
using NetStreams.Specs.Infrastructure.Models;

namespace NetStreams.Specs.Specifications.Component
{
    internal class NetStreamBuilderSpecs
    {
        //    [Subject("Configure")]
        //    class when_building_a_stream_using_configuration
        //    {
        //        static Action<INetStreamConfigurationBuilderContext> _configure;
        //        static ExpectedObject _expectedConfiguration;
        //        static ExpectedObject _expectedKafkaConsumerConfiguration;
        //        static ExpectedObject _expectedKafkaProducerConfiguration;
        //        static INetStream _stream;

        //        Establish context = () =>
        //        {
        //            _configure = cfg =>
        //            {
        //                cfg.BootstrapServers = "localhost:9021";
        //                cfg.ConsumerGroup = "consumergroup";
        //                cfg.SecurityProtocol = "SSL";
        //                cfg.SslCertificateLocation = "certloc.pem";
        //                cfg.SslCaLocation = "ca.crt";
        //                cfg.SslKeyLocation = "store.key";
        //                cfg.SslKeyPassword = "p4$$wurd";
        //            };

        //            var config = new NetStreamConfiguration();
        //            _configure(config);

        //            _expectedConfiguration = new
        //            {
        //                BootstrapServers = "localhost:9021",
        //                ConsumerGroup = "consumergroup",
        //                DeliveryMode = DeliveryMode.At_Least_Once,
        //                SecurityProtocol = "SSL",
        //                SslCertificateLocation = "certloc.pem",
        //                SslCaLocation = "ca.crt",
        //                SslKeyLocation = "store.key",
        //                SslKeyPassword = "p4$$wurd"
        //            }.ToExpectedObject();

        //            _expectedKafkaConsumerConfiguration = new
        //            {
        //                BootstrapServers = "localhost:9021",
        //                GroupId = "consumergroup",
        //                SecurityProtocol = SecurityProtocol.Ssl,
        //                SslCertificateLocation = "certloc.pem",
        //                SslCaLocation = "ca.crt",
        //                SslKeyLocation = "store.key",
        //                SslKeyPassword = "p4$$wurd"
        //            }.ToExpectedObject();

        //            _expectedKafkaProducerConfiguration = new
        //            {
        //                BootstrapServers = "localhost:9021",
        //                SecurityProtocol = SecurityProtocol.Ssl,
        //                SslCertificateLocation = "certloc.pem",
        //                SslCaLocation = "ca.crt",
        //                SslKeyLocation = "store.key",
        //                SslKeyPassword = "p4$$wurd"
        //            }.ToExpectedObject();
        //        };

        //        Because of = () => _stream = new NetStreamBuilder<string, TestMessage>(_configure).Stream("topic").Build();

        //        It should_set_configuration_values = () => _expectedConfiguration.ShouldMatch(_stream.Configuration);

        //        It should_map_config_values_to_kafka_consumer_config = () => _expectedKafkaConsumerConfiguration.ShouldMatch(_stream.Configuration.ToConsumerConfig());

        //        It should_map_config_values_to_kafka_producer_config = () => _expectedKafkaProducerConfiguration.ShouldMatch(_stream.Configuration.ToProducerConfig());
        //    }
        //}

        [Subject("Configure:Default")]
        class when_configuring_a_default_stream
        {
            static INetStream _stream;
            static Action<INetStreamConfigurationBuilderContext<string, TestMessage>> _configure;

            Establish context = () =>
            {
                _configure = cfg =>
                {
                    cfg.BootstrapServers = "localhost:9021";
                    cfg.ConsumerGroup = "consumergroup";
                };

                var config = new NetStreamConfiguration<string, TestMessage>();

                _configure(config);
            };

            Because of = () => _stream = new NetStreamBuilder<string, TestMessage>(_configure).Stream("topic").Build();

            It should_disable_auto_commit = () => _stream.Configuration.ToConsumerConfig().EnableAutoCommit.ShouldEqual(false);

            It should_set_auto_offset_reset_latest = () =>
                _stream.Configuration.ToConsumerConfig().AutoOffsetReset.ShouldEqual(AutoOffsetReset.Latest);
        }

        [Subject("Configure:DeliveryMode")]
        class when_configuring_an_at_most_once_stream
        {
            static INetStream _stream;
            static Action<INetStreamConfigurationBuilderContext<string, TestMessage>> _configure;

            Establish context = () =>
            {
                _configure = cfg =>
                {
                    cfg.BootstrapServers = "localhost:9021";
                    cfg.ConsumerGroup = "consumergroup";
                    cfg.DeliveryMode = DeliveryMode.At_Most_Once;
                };

                var config = new NetStreamConfiguration<string, TestMessage>();

                _configure(config);
            };

            Because of = () => _stream = new NetStreamBuilder<string, TestMessage>(_configure).Stream("topic").Build();

            It should_enable_auto_commit =
                () => _stream.Configuration.ToConsumerConfig().EnableAutoCommit.ShouldEqual(true);

            It should_set_auto_commit_interval = () =>
                _stream.Configuration.ToConsumerConfig().AutoCommitIntervalMs.ShouldEqual(5000);
        }

        [Subject("Configure:DeliveryMode")]
        class when_configuring_an_at_least_once_stream
        {
            static INetStream _stream;
            static Action<INetStreamConfigurationBuilderContext<string, TestMessage>> _configure;

            Establish context = () =>
            {
                _configure = cfg =>
                {
                    cfg.BootstrapServers = "localhost:9021";
                    cfg.ConsumerGroup = "consumergroup";
                    cfg.DeliveryMode = DeliveryMode.At_Least_Once;
                };

                var config = new NetStreamConfiguration<string, TestMessage>();

                _configure(config);
            };

            Because of = () => _stream = new NetStreamBuilder<string, TestMessage>(_configure).Stream("topic").Build();

            It should_disable_auto_commit =
                () => _stream.Configuration.ToConsumerConfig().EnableAutoCommit.ShouldEqual(false);
        }

        [Subject("Configure:DeliveryMode")]
        class when_configuring_a_custom_delivery_mode
        {
            static INetStream _stream;
            static Action<INetStreamConfigurationBuilderContext<string, TestMessage>> _configure;

            Establish context = () =>
            {
                _configure = cfg =>
                {
                    cfg.BootstrapServers = "localhost:9021";
                    cfg.ConsumerGroup = "consumergroup";
                    cfg.DeliveryMode = new DeliveryMode {EnableAutoCommit = false, AutoCommitIntervalMs = 10000};
                };

                var config = new NetStreamConfiguration<string, TestMessage>();

                _configure(config);
            };

            Because of = () => _stream = new NetStreamBuilder<string, TestMessage>(_configure).Stream("topic").Build();

            It should_set_enable_auto_commit = () =>
                _stream.Configuration.ToConsumerConfig().EnableAutoCommit.ShouldEqual(false);

            It should_set_auto_commit_interval_ms = () =>
                _stream.Configuration.ToConsumerConfig().AutoCommitIntervalMs.ShouldEqual(10000);
        }

        [Subject("Configure:DeliveryMode")]
        class when_configuring_auto_offset_reset
        {
            static INetStream _stream;
            static Action<INetStreamConfigurationBuilderContext<string, TestMessage>> _configure;

            Establish context = () =>
            {
                _configure = cfg =>
                {
                    cfg.BootstrapServers = "localhost:9021";
                    cfg.ConsumerGroup = "consumergroup";
                    cfg.AutoOffsetReset = AutoOffsetReset.Error;
                };

                var config = new NetStreamConfiguration<string, TestMessage>();

                _configure(config);
            };

            Because of = () => _stream = new NetStreamBuilder<string, TestMessage>(_configure).Stream("topic").Build();

            It should_set_auto_offset =
                () => _stream.Configuration.ToConsumerConfig().AutoOffsetReset.ShouldEqual(AutoOffsetReset.Error);
        }
    }
}