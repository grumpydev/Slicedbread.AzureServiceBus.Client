namespace Slicedbread.AzureServiceBus.Client
{
    public interface ISubscriber
    {
        bool CanProcess(MessageMetadata metadata);
    }
}