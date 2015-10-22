using Slicedbread.AzureServiceBus.Client.Serialisers;

namespace Slicedbread.AzureServiceBus.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.ServiceBus.Messaging;

    using ServiceBus;

    public class ServiceBusListener : IDisposable
    {
        private readonly ISubscriber[] subscribers;

        private readonly ISerialiser serialiser;

        private readonly IServiceBus serviceBus;

        /// <summary>
        /// Constructs a new instance of the service bus listener
        /// </summary>
        /// <param name="subscribers">Collection of subscribers</param>
        /// <param name="serialiser">Serialiser to use (defaults to SimpleJson)</param>
        /// <param name="serviceBus">Raw service bus (defaults to "real" service bus)</param>
        public ServiceBusListener(IEnumerable<ISubscriber> subscribers, ISerialiser serialiser = null, IServiceBus serviceBus = null)
        {
            this.subscribers = subscribers.ToArray();
            this.serialiser = serialiser ?? new SimpleJsonSerialiser();
            this.serviceBus = serviceBus ?? new ServiceBus.ServiceBus();
        }

        public void Connect(string connectionString, string queueName, QueueDescription queueDescription = null)
        {
            this.VerifyQueue(connectionString, queueName, queueDescription);

            this.serviceBus.Connect(connectionString, queueName);

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

        private async Task ProcessMessage(IServiceBusMessage serviceBusMessage)
        {
            var bodyString = await serviceBusMessage.GetMessageBody();
            dynamic body = this.serialiser.Deserialise(bodyString);

            var failed = false;

            foreach (var subscriber in this.subscribers.Where(subscriber => subscriber.CanProcess(serviceBusMessage)))
            {
                try
                {
                    await this.CallSubscriber(subscriber, body, serviceBusMessage);
                }
                catch (Exception)
                {
                    failed = true;
                    break;
                }
            }

            if (failed)
            {
                await serviceBusMessage.AbandonAsync();
            }
            else
            {
                await serviceBusMessage.CompleteAsync();
            }
        }

        private async Task CallSubscriber(ISubscriber subscriber, object body, IServiceBusMessage serviceBusMessage)
        {
            var dynamicSubscriber = subscriber as IDynamicSubscriber;

            if (dynamicSubscriber != null)
            {
                await dynamicSubscriber.Process(serviceBusMessage, body);
            }
        }

        private void VerifyQueue(string connectionString, string queueName, QueueDescription queueDescription)
        {
            this.serviceBus.VerifyQueue(connectionString, queueDescription ?? Defaults.GetDefaultQueueDescription(queueName));
        }
    }
}