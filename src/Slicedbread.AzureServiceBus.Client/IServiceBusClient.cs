using System.Threading.Tasks;

namespace Slicedbread.AzureServiceBus.Client
{
    using Microsoft.ServiceBus.Messaging;

    /// <summary>
    /// Client for sending message to the Azure Service Bus
    /// </summary>
    public interface IServiceBusClient
    {
        /// <summary>
        /// Connect to the service bus
        /// </summary>
        /// <param name="connectionString">Service bus connection string</param>
        /// <param name="queueName">Name of the queue to connect to</param>
        /// <param name="queueDescription">Optional queue description to use when on demand creating the queue</param>
        void Connect(string connectionString, string queueName, QueueDescription queueDescription = null);

        /// <summary>
        /// Send a message to the service bus
        /// </summary>
        /// <param name="messageType">Message type</param>
        /// <param name="payload">Dynamic payload object</param>
        /// <returns></returns>
        Task Send(string messageType, object payload);
    }
}