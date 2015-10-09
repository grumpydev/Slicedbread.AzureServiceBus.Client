using System.Threading.Tasks;

namespace Slicedbread.AzureServiceBus.Client.ServiceBus
{
    public interface IServiceBusMessage
    {
        string MessageType { get; }

        Task<string> GetMessageBody();

        Task AbandonAsync();

        Task CompleteAsync();
    }
}