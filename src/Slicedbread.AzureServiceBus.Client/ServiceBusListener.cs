using Slicedbread.AzureServiceBus.Client.Serialisers;

namespace Slicedbread.AzureServiceBus.Client
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Microsoft.ServiceBus.Messaging;

    using ServiceBus;

    public class ServiceBusListener : IDisposable
    {
        private readonly ISubscriber[] subscribers;

        private readonly ISerialiser serialiser;

        private readonly IServiceBus serviceBus;

        private readonly QueueDescription queueDescription;

        /// <summary>
        /// Constructs a new instance of the service bus listener
        /// </summary>
        /// <param name="subscribers">Collection of subscribers</param>
        /// <param name="serialiser">Serialiser to use (defaults to SimpleJson)</param>
        /// <param name="serviceBus">Raw service bus (defaults to "real" service bus)</param>
        /// <param name="queueDescription">Queue parameters to use if the queue does not exist</param>
        public ServiceBusListener(IEnumerable<ISubscriber> subscribers, ISerialiser serialiser = null, IServiceBus serviceBus = null, QueueDescription queueDescription = null)
        {
            this.subscribers = subscribers.ToArray();
            this.serialiser = serialiser ?? new SimpleJsonSerialiser();
            this.serviceBus = serviceBus ?? new ServiceBus.ServiceBus();
            this.queueDescription = queueDescription;
        }

        public void Connect(string connectionString, string queueName)
        {
            this.serviceBus.Connect(connectionString, queueName);

            this.VerifyQueue(connectionString, queueName);

            this.AddServiceBusSubscription();
        }

        public void Dispose()
        {
            this.serialiser.Dispose();
            this.serviceBus.Dispose();
        }

        private void AddServiceBusSubscription()
        {
            var options = new OnMessageOptions { AutoComplete = false, AutoRenewTimeout = TimeSpan.FromMinutes(1) };

            this.serviceBus.OnMessageAsync(this.ProcessMessage, options);
        }

        private async Task ProcessMessage(BrokeredMessage message)
        {
            var metaData = new MessageMetadata { MessageType = message.ContentType };
            var bodyString = await this.GetBodyString(message);
            dynamic body = this.serialiser.Deserialise(bodyString);

            foreach (var subscriber in this.subscribers.Where(subscriber => subscriber.CanProcess(metaData)))
            {
                await this.CallSubscriber(subscriber, body, bodyString, metaData);
            }
        }

        private async Task CallSubscriber(ISubscriber subscriber, object body, string bodyString, MessageMetadata metaData)
        {
            var dynamicSubscriber = subscriber as IDynamicSubscriber;

            if (dynamicSubscriber != null)
            {
                await dynamicSubscriber.Process(metaData, body);
            }
        }

        private async Task<string> GetBodyString(BrokeredMessage message)
        {
            var body = message.GetBody<Stream>();

            string bodyString;
            using (var reader = new StreamReader(body, Encoding.UTF8))
            {
                bodyString = await reader.ReadToEndAsync();
            }

            return bodyString;
        }

        private void VerifyQueue(string connectionString, string queueName)
        {
            this.serviceBus.VerifyQueue(connectionString, this.queueDescription ?? Defaults.GetDefaultQueueDescription(queueName));
        }
    }
}