using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Slicedbread.AzureServiceBus.Client.Serialisers;
using Slicedbread.AzureServiceBus.Client.ServiceBus;

namespace Slicedbread.AzureServiceBus.Client
{
    public class ServiceBusClient : IServiceBusClient
    {
        private readonly IServiceBus serviceBus;
        private readonly ISerialiser serialiser;

        /// <summary>
        /// Constructs a new instance of the service bus client
        /// </summary>
        /// <param name="serialiser">Serialiser to use (defaults to SimpleJson)</param>
        /// <param name="serviceBus">Raw service bus (defaults to "real" service bus)</param>
        public ServiceBusClient(ISerialiser serialiser = null, IServiceBus serviceBus = null)
        {
            this.serviceBus = serviceBus ?? new ServiceBus.ServiceBus();
            this.serialiser = serialiser ?? new SimpleJsonSerialiser();
        }

        public void Connect(string connectionString, string queueName, QueueDescription queueDescription = null)
        {
            this.VerifyQueue(connectionString, queueName, queueDescription);
            this.serviceBus.Connect(connectionString, queueName);
        }

        public async Task Send(string messageType, object payload)
        {
            var payloadString = this.serialiser.Serialise(payload);

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(payloadString));

            await this.serviceBus.Send(messageType, stream);
        }

        private void VerifyQueue(string connectionString, string queueName, QueueDescription queueDescription)
        {
            this.serviceBus.VerifyQueue(connectionString, queueDescription ?? Defaults.GetDefaultQueueDescription(queueName));
        }
    }
}