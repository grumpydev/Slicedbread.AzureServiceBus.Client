using Slicedbread.AzureServiceBus.Client.ServiceBus;

namespace Slicedbread.AzureServiceBus.Client
{
    public interface ISubscriber
    {
        bool CanProcess(IServiceBusMessage metadata);
    }
}