namespace Slicedbread.AzureServiceBus.Client.Serialisers
{
    public class SimpleJsonSerialiser : ISerialiser
    {
        public string Serialise(object payload)
        {
            return SimpleJson.SerializeObject(payload);
        }

        public dynamic Deserialise(string payload)
        {
            return SimpleJson.DeserializeObject(payload);
        }

        public void Dispose()
        {
        }
    }
}