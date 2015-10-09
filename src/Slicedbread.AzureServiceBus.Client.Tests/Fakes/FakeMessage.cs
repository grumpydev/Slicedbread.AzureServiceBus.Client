using System.Threading.Tasks;
using Slicedbread.AzureServiceBus.Client.ServiceBus;

namespace Slicedbread.AzureServiceBus.Client.Tests.Fakes
{
    public class FakeMessage : IServiceBusMessage
    {
        private readonly string body;

        public string MessageType { get; private set;  }

        public bool Abandoned { get; set; }

        public bool Completed { get; set; }

        public FakeMessage(string messageType, string body)
        {
            this.MessageType = messageType;
            this.body = body;
        }

        public Task<string> GetMessageBody()
        {
            return Task.FromResult(this.body);
        }

        public Task AbandonAsync()
        {
            this.Abandoned = true;

            return Task.FromResult(0);
        }

        public Task CompleteAsync()
        {
            this.Completed = true;

            return Task.FromResult(0);
        }
    }
}