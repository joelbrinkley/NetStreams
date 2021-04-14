using ExpectedObjects;
using Machine.Specifications;
using NetStreams.Specs.Infrastructure;
using System;
using NetStreams.Configuration;
using NetStreams.Specs.Infrastructure.Models;

namespace NetStreams.Specs.Specifications.Component
{
    class NetStreamBuilderSpecs
    {
        [Subject("Configure")]
        class when_building_a_stream_using_configuration
        {
            static Action<INetStreamConfigurationBuilderContext> _configure;
            static ExpectedObject _expectedConfiguration;
            static INetStream<string, TestMessage> _stream;

            Establish context = () =>
            {
                _configure = cfg =>
                {
                    cfg.BootstrapServers = "localhost:9021";
                    cfg.ConsumerGroup = "consumergroup";
                };

                var config = new NetStreamConfiguration();
                _configure(config);

                _expectedConfiguration = new
                {
                    BootstrapServers = "localhost:9021",
                    ConsumerGroup = "consumergroup"
                }.ToExpectedObject();
            };

            Because of = () => _stream = new NetStreamBuilder(_configure).Stream<string, TestMessage>("topic");

            It should_set_configuration_values = () => _expectedConfiguration.ShouldMatch(_stream.Configuration);
        }
    }
}
