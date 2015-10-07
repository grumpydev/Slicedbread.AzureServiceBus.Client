namespace Slicedbread.AzureServiceBus.Client
{
    using System;

    public interface ISerialiser : IDisposable
    {
        string Serialise(object payload);

        dynamic Deserialise(string payload);

        TDestination Deserialise<TDestination>(string payload);
    }
}