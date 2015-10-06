using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace Slicedbread.AzureServiceBus.Client.ServiceBus
{
    public interface IServiceBus : IDisposable
    {
        void Connect(string connectionString, string queueName);

        void VerifyQueue(string connectionString, QueueDescription description);

        Task Send(string messageType, Stream payload);

        void OnMessageAsync(Func<BrokeredMessage, Task> callback, OnMessageOptions options);
    }
}