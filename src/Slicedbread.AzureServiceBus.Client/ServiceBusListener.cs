namespace Slicedbread.AzureServiceBus.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Slicedbread.AzureServiceBus.Client.ServiceBus;

    public class ServiceBusListener : IDisposable
    {
        private readonly string connectionString;

        private readonly string queueName;

        private readonly ISubscriber[] subscribers;

        private readonly ISerialiser serialiser;

        private readonly IServiceBus serviceBus;

        public ServiceBusListener(string connectionString, string queueName, IEnumerable<ISubscriber> subscribers, ISerialiser serialiser, IServiceBus serviceBus)
        {
            this.connectionString = connectionString;
            this.queueName = queueName;
            this.subscribers = subscribers.ToArray();
            this.serialiser = serialiser;
            this.serviceBus = serviceBus;
        }

        public void Dispose()
        {
            this.serialiser.Dispose();
            this.serviceBus.Dispose();
        }
    }
}