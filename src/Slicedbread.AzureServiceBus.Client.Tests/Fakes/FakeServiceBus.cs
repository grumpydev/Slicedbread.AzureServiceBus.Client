using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Slicedbread.AzureServiceBus.Client.ServiceBus;

namespace Slicedbread.AzureServiceBus.Client.Tests.Fakes
{
    public class FakeServiceBus : IServiceBus
    {
        public string QueueName { get; set; }

        public string ConnectionString { get; set; }

        public bool Disposed { get; set; }

        public QueueDescription VerifyQueueDescription { get; set; }

        public string VerifyQueueConnectionString { get; set; }

        public Stream SentPayloadStream { get; set; }

        public string SentMessageType { get; set; }

        public OnMessageOptions CallbackOptions { get; set; }

        public Func<IServiceBusMessage, Task> CallBack { get; set; }

        public void Dispose()
        {
            this.Disposed = true;
        }

        public void Connect(string connectionString, string queueName)
        {
            this.ConnectionString = connectionString;
            this.QueueName = queueName;
        }

        public void VerifyQueue(string connectionString, QueueDescription description)
        {
            this.VerifyQueueConnectionString = connectionString;
            this.VerifyQueueDescription = description;
        }

        public Task Send(string messageType, Stream payload)
        {
            this.SentMessageType = messageType;
            this.SentPayloadStream = payload;

            return Task.FromResult(0);
        }

        public void OnMessageAsync(Func<IServiceBusMessage, Task> callback, OnMessageOptions options)
        {
            this.CallBack = callback;
            this.CallbackOptions = options;
        }
    }
}