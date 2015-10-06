using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace Slicedbread.AzureServiceBus.Client.ServiceBus
{
    public class ServiceBus : IServiceBus
    {
        private QueueClient _client;

        public void Connect(string connectionString, string queueName)
        {
            this._client = QueueClient.CreateFromConnectionString(connectionString, queueName);
        }

        public void VerifyQueue(string connectionString, QueueDescription description)
        {
            var namespaceManager =
                NamespaceManager.CreateFromConnectionString(connectionString);

            if (!namespaceManager.QueueExists(description.Path))
            {
                namespaceManager.CreateQueue(description);
            }
        }

        public async Task Send(string messageType, Stream payload)
        {
            if (this._client == null)
            {
                throw new InvalidOperationException("Client is not connected.");
            }

            var message = new BrokeredMessage(payload, true) { ContentType = messageType }; 

            await this._client.SendAsync(message);
        }

        public void OnMessageAsync(Func<BrokeredMessage, Task> callback, OnMessageOptions options)
        {
            if (this._client == null)
            {
                throw new InvalidOperationException("Client is not connected.");
            }

            this._client.OnMessageAsync(callback, options);
        }

        public void Dispose()
        {
            if (this._client != null)
            {
                this._client.Close();
            }
        }
    }
}