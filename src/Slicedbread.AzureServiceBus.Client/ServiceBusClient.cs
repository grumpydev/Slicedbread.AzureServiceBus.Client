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
        private readonly QueueDescription queueDescription;

        public ServiceBusClient(ISerialiser serialiser = null, IServiceBus bus = null, QueueDescription queueDescription = null)
        {
            this.serviceBus = bus ?? new ServiceBus.ServiceBus();
            this.serialiser = serialiser ?? new SimpleJsonSerialiser();
            this.queueDescription = queueDescription;
        }   

        public void Connect(string connectionString, string queueName)
        {
            this.serviceBus.Connect(connectionString, queueName);

            this.VerifyQueue(connectionString, queueName);
        }

        public async Task Send(string messageType, object payload)
        {
            var payloadString = this.serialiser.Serialise(payload);

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(payloadString));

            await this.serviceBus.Send(messageType, stream);
        }

        private void VerifyQueue(string connectionString, string queueName)
        {
            this.serviceBus.VerifyQueue(connectionString, this.queueDescription ?? Defaults.GetDefaultQueueDescription(queueName));
        }
    }
}