using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.ServiceBus.Messaging;
using Should;
using Slicedbread.AzureServiceBus.Client.Serialisers;
using Slicedbread.AzureServiceBus.Client.Tests.Fakes;
using Xunit;

namespace Slicedbread.AzureServiceBus.Client.Tests
{
    public class ServiceBusClientFixture
    {
        private FakeServiceBus bus;

        private string connectionString;

        private string queueName;

        private ISerialiser serialiser;

        public ServiceBusClientFixture()
        {
            this.bus = new FakeServiceBus();
            this.connectionString = "ConnectionString";
            this.queueName = "queuename";
            this.serialiser = A.Fake<ISerialiser>();
        }

        [Fact]
        public void Should_verify_queue_on_connect()
        {
            // Given
            var listener = new ServiceBusClient(
                this.serialiser,
                this.bus);

            // When
            listener.Connect(this.connectionString, this.queueName);

            // Then
            this.bus.VerifyQueueConnectionString.ShouldEqual(this.connectionString);
            this.bus.VerifyQueueDescription.ShouldNotBeNull();
            this.bus.VerifyQueueDescription.Path.ShouldEqual(this.queueName);
        }

        [Fact]
        public void Should_use_passed_in_queue_parameters_if_specified()
        {
            // Given
            var description = new QueueDescription(this.queueName);
            var listener = new ServiceBusClient(
                this.serialiser,
                this.bus,
                description);

            // When
            listener.Connect(this.connectionString, this.queueName);

            // Then
            this.bus.VerifyQueueDescription.ShouldBeSameAs(description);
        }

        [Fact]
        public void Should_serialise_message()
        {
            // Given
            var description = new QueueDescription(this.queueName);
            var listener = new ServiceBusClient(
                this.serialiser,
                this.bus,
                description);
            listener.Connect(this.connectionString, this.queueName);
            var payload = new Object();
            A.CallTo(() => this.bus.Send(A<string>._, A<Stream>._))
             .Returns(Task.FromResult(0));

            // When
            listener.Send("messageType", payload);

            // Then
            A.CallTo(() => this.serialiser.Serialise(payload))
             .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_set_message_type()
        {
            // Given
            var messageType = string.Empty;
            var description = new QueueDescription(this.queueName);
            var listener = new ServiceBusClient(
                this.serialiser,
                this.bus,
                description);
            listener.Connect(this.connectionString, this.queueName);
            var payload = new Object();
            A.CallTo(() => this.bus.Send(A<string>._, A<Stream>._))
             .Invokes(foc => messageType = (string)foc.Arguments[0])
             .Returns(Task.FromResult(0));

            // When
            listener.Send("messageType", payload);

            // Then
            messageType.ShouldEqual("messageType");
        }

        [Fact]
        public void Should_send_stream_from_serialisation_to_servicebus()
        {
            // Given
            var streamContents = string.Empty;
            var description = new QueueDescription(this.queueName);
            var listener = new ServiceBusClient(
                this.serialiser,
                this.bus,
                description);
            listener.Connect(this.connectionString, this.queueName);
            var payload = new Object();
            A.CallTo(() => this.serialiser.Serialise(payload))
             .Returns("FooBarBaz");
            A.CallTo(() => this.bus.Send(A<string>._, A<Stream>._))
             .Invokes(foc => streamContents = this.GetStreamContents((Stream)foc.Arguments[1]))
             .Returns(Task.FromResult(0));

            // When
            listener.Send("messageType", payload);

            // Then
            streamContents.ShouldEqual("FooBarBaz");
        }

        private string GetStreamContents(Stream stream)
        {
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
               return reader.ReadToEnd();
            }
        }
    }
}