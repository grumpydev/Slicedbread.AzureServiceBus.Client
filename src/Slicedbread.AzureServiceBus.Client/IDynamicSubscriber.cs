using Slicedbread.AzureServiceBus.Client.ServiceBus;

namespace Slicedbread.AzureServiceBus.Client
{
    using System.Threading.Tasks;

    public interface IDynamicSubscriber : ISubscriber
    {
        Task Process(IServiceBusMessage serviceBusMessage, dynamic body);
    }
}