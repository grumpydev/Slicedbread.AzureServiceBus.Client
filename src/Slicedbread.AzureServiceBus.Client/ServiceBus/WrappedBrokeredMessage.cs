using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace Slicedbread.AzureServiceBus.Client.ServiceBus
{
    public class WrappedBrokeredMessage : IServiceBusMessage
    {
        private readonly BrokeredMessage wrappedMessage;

        public string MessageType { get { return this.wrappedMessage.ContentType; } }

        public WrappedBrokeredMessage(BrokeredMessage wrappedMessage)
        {
            this.wrappedMessage = wrappedMessage;
        }

        public async Task<string> GetMessageBody()
        {
            var body = this.wrappedMessage.GetBody<Stream>();

            string bodyString;
            using (var reader = new StreamReader(body, Encoding.UTF8))
            {
                bodyString = await reader.ReadToEndAsync();
            }

            return bodyString;
        }

        public Task AbandonAsync()
        {
            return this.wrappedMessage.AbandonAsync();
        }

        public Task CompleteAsync()
        {
            return this.wrappedMessage.CompleteAsync();
            throw new System.NotImplementedException();
        }
    }
}