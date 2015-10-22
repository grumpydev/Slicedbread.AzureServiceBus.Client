using System;
using Microsoft.ServiceBus.Messaging;

namespace Slicedbread.AzureServiceBus.Client
{
    public static class Defaults
    {
        public static QueueDescription GetDefaultQueueDescription(string queueName)
        {
            return new QueueDescription(queueName)
            {
                MaxSizeInMegabytes = 5120,
                DefaultMessageTimeToLive = new TimeSpan(7, 0, 0, 0),
                MaxDeliveryCount = 3
            };
        }
    }
}