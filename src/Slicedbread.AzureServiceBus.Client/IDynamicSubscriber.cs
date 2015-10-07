namespace Slicedbread.AzureServiceBus.Client
{
    using System.Threading.Tasks;

    public interface IDynamicSubscriber : ISubscriber
    {
        Task Process(MessageMetadata metadata, dynamic body);
    }
}