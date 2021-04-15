using System;
using System.Text;
using Confluent.Kafka;
using Machine.Specifications;
using NetStreams.Internal;
using NetStreams.Serialization;
using NetStreams.Specs.Infrastructure.Models;
using Newtonsoft.Json;

namespace NetStreams.Specs.Specifications.Component
{
    internal class HeaderSerializationStrategySpecs
    {
        [Subject("Serialization")]
        class when_deserializing_a_serialization_context_with_a_netstreams_header
        {
            static HeaderSerializationStrategy<TestMessage> _serializer;
            static SerializationContext _serializationContext;
            static TestMessage _childTestMessage;
            static TestMessage _expectedResult;

            Establish context = () =>
            {
                _childTestMessage = new ChildTestMessage();
                _serializationContext = new SerializationContext(
                    MessageComponentType.Value,
                    Guid.NewGuid().ToString(),
                    new Headers
                    {
                        new Header(NetStreamConstants.HEADER_TYPE,
                            Encoding.UTF8.GetBytes(_childTestMessage.GetType().AssemblyQualifiedName ?? string.Empty))
                    });

                _serializer = new HeaderSerializationStrategy<TestMessage>();
            };

            Because of = () =>
                _expectedResult = _serializer.Deserialize(
                    new ReadOnlySpan<byte>(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(_childTestMessage))), false,
                    _serializationContext);

            It should_deserialize_to_the_namespace_in_the_header = () => _expectedResult.GetType().ShouldEqual(typeof(ChildTestMessage));
        }


        [Subject("Serialization")]
        class when_deserializing_a_serialization_context_with_a_missing_header
        {
            static HeaderSerializationStrategy<TestMessage> _serializer;
            static SerializationContext _serializationContext;
            static TestMessage _childTestMessage;
            static TestMessage _expectedResult;

            Establish context = () =>
            {
                _childTestMessage = new ChildTestMessage();
                _serializationContext = new SerializationContext(
                    MessageComponentType.Value,
                    Guid.NewGuid().ToString(),
                    new Headers());

                _serializer = new HeaderSerializationStrategy<TestMessage>();
            };

            Because of = () =>
                _expectedResult = _serializer.Deserialize(
                    new ReadOnlySpan<byte>(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(_childTestMessage))), false,
                    _serializationContext);

            It should_deserialize_to_the_namespace_in_the_header = () => _expectedResult.GetType().ShouldEqual(typeof(TestMessage));
        }

        [Subject("Serialization")]
        class when_deserializing_with_null
        {
            static HeaderSerializationStrategy<int> _serializer;
            static SerializationContext _serializationContext;
            static int _expectedResult;

            Establish context = () =>
            {
                _serializationContext = new SerializationContext(
                    MessageComponentType.Value,
                    Guid.NewGuid().ToString(),
                    new Headers());

                _serializer = new HeaderSerializationStrategy<int>();
            };

            Because of = () =>
                _expectedResult = _serializer.Deserialize(
                    new ReadOnlySpan<byte>(Encoding.UTF8.GetBytes(string.Empty)), true, _serializationContext);

            It should_deserialize_to_the_default = () => _expectedResult.ShouldEqual(0);
        }
    }
}