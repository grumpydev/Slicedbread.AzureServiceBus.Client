namespace Slicedbread.AzureServiceBus.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.ServiceBus.Messaging;

    using Slicedbread.AzureServiceBus.Client.ServiceBus;

    public class ServiceBusListener : IDisposable
    {
        private readonly string connectionString;

        private readonly string queueName;

        private readonly ISubscriber[] subscribers;

        private readonly ISerialiser serialiser;

        private readonly IServiceBus serviceBus;

        public ServiceBusListener(string connectionString, string queueName, IEnumerable<ISubscriber> subscribers, ISerialiser serialiser, IServiceBus serviceBus, QueueDescription queueDescription = null)
        {
            this.connectionString = connectionString;
            this.queueName = queueName;
            this.subscribers = subscribers.ToArray();
            this.serialiser = serialiser;
            this.serviceBus = serviceBus;

            this.VerifyQueue(connectionString, queueName, queueDescription);

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

            foreach (var subscriber in this.subscribers.Where(subscriber => subscriber.CanProcess(metaData)))
            {
                
            }
        }

        private void VerifyQueue(string connectionString, string queueName, QueueDescription queueDescription)
        {
            this.serviceBus.VerifyQueue(connectionString, queueDescription ?? this.GetDefaultQueueDescription(queueName));
        }

        private QueueDescription GetDefaultQueueDescription(string queueName)
        {
            return new QueueDescription(queueName)
                         {
                             MaxSizeInMegabytes = 5120,
                             DefaultMessageTimeToLive = new TimeSpan(30, 0, 0)
                         };;
        }
    }
}