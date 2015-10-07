namespace Slicedbread.AzureServiceBus.Client.Tests
{
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;

    using FakeItEasy;

    using Microsoft.ServiceBus.Messaging;

    using Should;

    using Slicedbread.AzureServiceBus.Client.Tests.Fakes;

    using Xunit;

    public class ServiceBusListenerFixture
    {
        private FakeServiceBus bus;

        private string connectionString;

        private string queueName;

        private ISerialiser serialiser;

        public ServiceBusListenerFixture()
        {
            this.bus = new FakeServiceBus();
            this.connectionString = "ConnectionString";
            this.queueName = "queuename";
            this.serialiser = A.Fake<ISerialiser>();
        }

        [Fact]
        public void Should_verify_queue_on_startup()
        {
            // Given
            // When
            var listener = new ServiceBusListener(
                this.connectionString,
                this.queueName,
                Enumerable.Empty<ISubscriber>(),
                this.serialiser,
                this.bus);

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

            // When
            var listener = new ServiceBusListener(
                this.connectionString,
                this.queueName,
                Enumerable.Empty<ISubscriber>(),
                this.serialiser,
                this.bus,
                description);

            // Then
            this.bus.VerifyQueueDescription.ShouldBeSameAs(description);
        }

        [Fact]
        public void Should_call_can_process_on_each_subscriber()
        {
            // Given
            var subscriberOne = A.Fake<ISubscriber>();
            var subscriberTwo = A.Fake<ISubscriber>();
            var listener = new ServiceBusListener(
                this.connectionString,
                this.queueName,
                new[] { subscriberOne, subscriberTwo },
                this.serialiser,
                this.bus);
            var message = this.GetBrokeredMessage("foo");

            // When
            this.bus.CallBack.Invoke(message).Wait();

            // Then
            A.CallTo(() => subscriberOne.CanProcess(A<MessageMetadata>._))
             .MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => subscriberTwo.CanProcess(A<MessageMetadata>._))
             .MustHaveHappened(Repeated.Exactly.Once);
        }

        private BrokeredMessage GetBrokeredMessage(string messageType, string body = null)
        {
            var bodyStream = new MemoryStream(Encoding.UTF8.GetBytes(body ?? string.Empty));

            return new BrokeredMessage(bodyStream, true) { ContentType = messageType };
        }
    }
}