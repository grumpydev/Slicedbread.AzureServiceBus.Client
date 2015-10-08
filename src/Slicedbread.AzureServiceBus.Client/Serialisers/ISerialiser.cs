using System;

namespace Slicedbread.AzureServiceBus.Client.Serialisers
{
    public interface ISerialiser : IDisposable
    {
        string Serialise(object payload);

        dynamic Deserialise(string payload);
    }
}