using System;
using Machine.Specifications;
using NetStreams.Configuration;
using NetStreams.Logging;
using NetStreams.Specs.Infrastructure.Mocks;
using NetStreams.Specs.Infrastructure.Models;

namespace NetStreams.Specs.Specifications.Component
{
    internal class LoggingSpecs
    {
        [Subject("Logging")]
        class when_configuring_a_logger
        {
            static INetStream _stream;
            static Action<INetStreamConfigurationBuilderContext<string, TestMessage>> _configure;

            static MockLogger _mockLogger;
            static string _expectedMessage;
            static NetStreamConfiguration<string, TestMessage> _config;

            Establish context = () =>
            {
                _mockLogger = new MockLogger();

                _configure = cfg =>
                {
                    cfg.BootstrapServers = "localhost:9021";
                    cfg.ConsumerGroup = "consumergroup";
                    cfg.ConfigureLogging(logging =>
                    {
                        logging.AddLogger(_mockLogger);
                        logging.AddConsole();
                    });
                };

                _config = new NetStreamConfiguration<string, TestMessage>();

                _configure(_config);

                _expectedMessage = "TestLogMessage";
            };

            Because of = () => _config.Logger.Write(_expectedMessage);

            It should_write = () => _mockLogger.Messages.ShouldContain(_expectedMessage);
        }
    }
}