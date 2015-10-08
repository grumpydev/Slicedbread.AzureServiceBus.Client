using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Slicedbread.AzureServiceBus.Client.Serialisers;
using Slicedbread.AzureServiceBus.Client.ServiceBus;

namespace Slicedbread.AzureServiceBus.Client
{
    public class ServiceBusClient : IServiceBusClient
    {
        private readonly IServiceBus bus;
        private readonly ISerialiser serialiser;
        private readonly QueueDescription queueDescription;

        public ServiceBusClient(IServiceBus bus, ISerialiser serialiser = null, QueueDescription queueDescription = null)
        {
            this.bus = bus;
            this.serialiser = serialiser ?? new SimpleJsonSerialiser();
            this.queueDescription = queueDescription;
        }

        public void Connect(string connectionString, string queueName)
        {
            throw new System.NotImplementedException();
        }

        public Task Send(object payload)
        {
            throw new System.NotImplementedException();
        }
    }
}