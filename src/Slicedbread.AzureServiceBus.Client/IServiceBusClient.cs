using System.Threading.Tasks;

namespace Slicedbread.AzureServiceBus.Client
{
    public interface IServiceBusClient
    {
        void Connect(string connectionString, string queueName);

        Task Send(string messageType, object payload);
    }
}