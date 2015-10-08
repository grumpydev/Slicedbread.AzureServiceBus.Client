using System.Threading.Tasks;
using Slicedbread.AzureServiceBus.Client.Serialisers;

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
        public void Should_verify_queue_on_connect()
        {
            // Given
            var listener = new ServiceBusListener(
                Enumerable.Empty<ISubscriber>(),
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
            var listener = new ServiceBusListener(
                Enumerable.Empty<ISubscriber>(),
                this.serialiser,
                this.bus,
                description);

            // When
            listener.Connect(this.connectionString, this.queueName);

            // Then
            this.bus.VerifyQueueDescription.ShouldBeSameAs(description);
        }

        [Fact]
        public void Should_connect()
        {
            // Given
            var listener = new ServiceBusListener(
                Enumerable.Empty<ISubscriber>(),
                this.serialiser,
                this.bus);

            // When
            listener.Connect(this.connectionString, this.queueName);

            // Then
            this.bus.ConnectionString.ShouldEqual(this.connectionString);
            this.bus.QueueName.ShouldEqual(this.queueName);
        }

        [Fact]
        public void Should_call_can_process_on_each_subscriber()
        {
            // Given
            var subscriberOne = A.Fake<ISubscriber>();
            var subscriberTwo = A.Fake<ISubscriber>();
            var listener = new ServiceBusListener(
                new[] { subscriberOne, subscriberTwo },
                this.serialiser,
                this.bus);
            var message = this.GetBrokeredMessage("foo");
            listener.Connect(this.connectionString, this.queueName);

            // When
            this.bus.CallBack.Invoke(message).Wait();

            // Then
            A.CallTo(() => subscriberOne.CanProcess(A<MessageMetadata>._))
             .MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => subscriberTwo.CanProcess(A<MessageMetadata>._))
             .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_only_call_subscriber_who_can_process()
        {
            // Given
            var subscriberOne = A.Fake<IDynamicSubscriber>();
            A.CallTo(() => subscriberOne.CanProcess(A<MessageMetadata>._)).Returns(true);
            var subscriberTwo = A.Fake<IDynamicSubscriber>();
            A.CallTo(() => subscriberTwo.CanProcess(A<MessageMetadata>._)).Returns(false);
            var listener = new ServiceBusListener(
                new[] { subscriberOne, subscriberTwo },
                this.serialiser,
                this.bus);
            var message = this.GetBrokeredMessage("foo");
            listener.Connect(this.connectionString, this.queueName);

            // When
            this.bus.CallBack.Invoke(message).Wait();

            // Then
            A.CallTo(() => subscriberOne.Process(A<MessageMetadata>._, A<object>._))
             .MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => subscriberTwo.Process(A<MessageMetadata>._, A<object>._))
             .MustNotHaveHappened();
        }

        [Fact]
        public void Should_deserialise_dynamically()
        {
            // Given
            var subscriberOne = A.Fake<IDynamicSubscriber>();
            string foo = null;
            long baz = 0;
            A.CallTo(() => subscriberOne.CanProcess(A<MessageMetadata>._))
             .Returns(true);
            A.CallTo(() => subscriberOne.Process(A<MessageMetadata>._, A<object>._))
                .Invokes(foc =>
                {
                    dynamic model = foc.Arguments[1];

                    foo = model.Foo;
                    baz = model.Baz;
                }).Returns(Task.FromResult(0));
            var listener = new ServiceBusListener(
                new[] { subscriberOne },
                new SimpleJsonSerialiser(), 
                this.bus);
            var message = this.GetBrokeredMessage("foo", "{ \"Foo\" : \"Bar\", \"Baz\" : 23 }");
            listener.Connect(this.connectionString, this.queueName);

            // When
            this.bus.CallBack.Invoke(message).Wait();

            // Then
            foo.ShouldEqual("Bar");
            baz.ShouldEqual(23);
        }

        private BrokeredMessage GetBrokeredMessage(string messageType, string body = null)
        {
            var bodyStream = new MemoryStream(Encoding.UTF8.GetBytes(body ?? string.Empty));

            return new BrokeredMessage(bodyStream, true) { ContentType = messageType };
        }
    }
}