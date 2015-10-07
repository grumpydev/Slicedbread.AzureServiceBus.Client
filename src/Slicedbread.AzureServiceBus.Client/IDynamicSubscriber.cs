namespace Slicedbread.AzureServiceBus.Client
{
    using System.ServiceModel.Channels;

    public interface ISubscriber
    {
        
    }

    public interface IDynamicSubscriber : ISubscriber
    {
        bool CanProcess(MessageMetadata metadata);

        void Process(MessageMetadata metadata, dynamic body);
    }

    public interface ITypedSubscriber<in TPayload> : ISubscriber
    {
        bool CanProcess(Message metadata);

        void Process(MessageMetadata metadata, TPayload body);
    }
}