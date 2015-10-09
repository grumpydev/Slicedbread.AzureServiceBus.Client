using System;
using System.IO;
using System.Text;
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

        public void OnMessageAsync(Func<IServiceBusMessage, Task> callback, OnMessageOptions options)
        {
            if (this._client == null)
            {
                throw new InvalidOperationException("Client is not connected.");
            }

            this._client.OnMessageAsync(m => this.UnWrapMessage(m, callback), options);
        }

        private async Task UnWrapMessage(BrokeredMessage brokeredMessage, Func<IServiceBusMessage, Task> callback)
        {
            var message = new WrappedBrokeredMessage(brokeredMessage);

            await callback.Invoke(message);
        }

        public void Dispose()
        {
            if (this._client != null)
            {
                this._client.Close();
            }
        }

        private async Task<string> GetBody(BrokeredMessage message)
        {
            var body = message.GetBody<Stream>();

            string bodyString;
            using (var reader = new StreamReader(body, Encoding.UTF8))
            {
                bodyString = await reader.ReadToEndAsync();
            }

            return bodyString;
        }
    }
}