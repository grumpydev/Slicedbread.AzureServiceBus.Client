using System.Linq;
using FakeItEasy;
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
    }
}