﻿using System;
using Confluent.Kafka;
using ExpectedObjects;
using Machine.Specifications;
using NetStreams.Configuration;
using NetStreams.Configuration.Internal;
using NetStreams.Specs.Infrastructure.Models;

namespace NetStreams.Specs.Specifications.Component
{
    internal class NetStreamBuilderSpecs
    {
        [Subject("Configure")]
        class when_building_a_stream_using_configuration
        {
            static Action<INetStreamConfigurationBuilderContext> _configure;
            static ExpectedObject _expectedConfiguration;
            static ExpectedObject _expectedKafkaConsumerConfiguration;
            static ExpectedObject _expectedKafkaProducerConfiguration;
            static INetStream<string, TestMessage> _stream;

            Establish context = () =>
            {
                _configure = cfg =>
                {
                    cfg.BootstrapServers = "localhost:9021";
                    cfg.ConsumerGroup = "consumergroup";
                    cfg.SecurityProtocol = "SSL";
                    cfg.SslCertificateLocation = "broker.certificate.pem";
                    cfg.SslCaLocation = "snakeoil-ca-1.crt";
                    cfg.SslKeystoreLocation = "broker.key";
                };

                var config = new NetStreamConfiguration();
                _configure(config);

                _expectedConfiguration = new
                {
                    BootstrapServers = "localhost:9021",
                    ConsumerGroup = "consumergroup",
                    DeliveryMode = DeliveryMode.At_Least_Once,
                    SecurityProtocol = "SSL",
                    SslCertificateLocation = "broker.certificate.pem",
                    SslCaLocation = "snakeoil-ca-1.crt",
                    SslKeystoreLocation = "broker.key",
                }.ToExpectedObject();

                _expectedKafkaConsumerConfiguration = new
                {
                    BootstrapServers = "localhost:9021",
                    GroupId = "consumergroup",
                    SecurityProtocol = SecurityProtocol.Ssl,
                    SslCertificateLocation = "broker.certificate.pem",
                    SslCaLocation = "snakeoil-ca-1.crt",
                    SslKeystoreLocation = "broker.key",
                }.ToExpectedObject();

                _expectedKafkaProducerConfiguration = new
                {
                    BootstrapServers = "localhost:9021",
                    SecurityProtocol = SecurityProtocol.Ssl,
                    SslCertificateLocation = "broker.certificate.pem",
                    SslCaLocation = "snakeoil-ca-1.crt",
                    SslKeystoreLocation = "broker.key",
                }.ToExpectedObject();
            };

            Because of = () => _stream = new NetStreamBuilder(_configure).Stream<string, TestMessage>("topic");

            It should_set_configuration_values = () => _expectedConfiguration.ShouldMatch(_stream.Configuration);

            It should_map_config_values_to_kafka_consumer_config = () => _expectedKafkaConsumerConfiguration.ShouldMatch(_stream.Configuration.ToConsumerConfig());

            It should_map_config_values_to_kafka_producer_config = () => _expectedKafkaProducerConfiguration.ShouldMatch(_stream.Configuration.ToProducerConfig());
        }
    }

    [Subject("Configure:DeliveryMode")]
    class when_configuring_an_at_most_once_stream
    {
        static INetStream<string, TestMessage> _stream;
        static Action<INetStreamConfigurationBuilderContext> _configure;

        Establish context = () =>
        {
            _configure = cfg =>
            {
                cfg.BootstrapServers = "localhost:9021";
                cfg.ConsumerGroup = "consumergroup";
                cfg.DeliveryMode = DeliveryMode.At_Most_Once;
            };

            var config = new NetStreamConfiguration();

            _configure(config);
        };

        Because of = () => _stream = new NetStreamBuilder(_configure).Stream<string, TestMessage>("topic");

        It should_enable_auto_commit = () => _stream.Configuration.ToConsumerConfig().EnableAutoCommit.ShouldEqual(true);

        It should_set_auto_commit_interval = () => _stream.Configuration.ToConsumerConfig().AutoCommitIntervalMs.ShouldEqual(5000);
    }

    [Subject("Configure:DeliveryMode")]
    class when_configuring_an_at_least_once_stream
    {
        static INetStream<string, TestMessage> _stream;
        static Action<INetStreamConfigurationBuilderContext> _configure;

        Establish context = () =>
        {
            _configure = cfg =>
            {
                cfg.BootstrapServers = "localhost:9021";
                cfg.ConsumerGroup = "consumergroup";
                cfg.DeliveryMode = DeliveryMode.At_Least_Once;
            };

            var config = new NetStreamConfiguration();

            _configure(config);
        };

        Because of = () => _stream = new NetStreamBuilder(_configure).Stream<string, TestMessage>("topic");

        It should_disable_auto_commit = () => _stream.Configuration.ToConsumerConfig().EnableAutoCommit.ShouldEqual(false);
    }

    [Subject("Configure:DeliveryMode")]
    class when_configuring_a_custom_delivery_mode
    {
        static INetStream<string, TestMessage> _stream;
        static Action<INetStreamConfigurationBuilderContext> _configure;

        Establish context = () =>
        {
            _configure = cfg =>
            {
                cfg.BootstrapServers = "localhost:9021";
                cfg.ConsumerGroup = "consumergroup";
                cfg.DeliveryMode = new DeliveryMode {EnableAutoCommit = false, AutoCommitIntervalMs = 10000};
            };

            var config = new NetStreamConfiguration();

            _configure(config);
        };

        Because of = () => _stream = new NetStreamBuilder(_configure).Stream<string, TestMessage>("topic");

        It should_set_enable_auto_commit = () => _stream.Configuration.ToConsumerConfig().EnableAutoCommit.ShouldEqual(false);

        It should_set_auto_commit_interval_ms = () => _stream.Configuration.ToConsumerConfig().AutoCommitIntervalMs.ShouldEqual(10000);
    }
}