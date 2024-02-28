namespace Tenon.Serialization.Abstractions
{
    public interface ISerializer
    {
        string SerializeObject(object value);

        byte[] Serialize<T>(T value);

        T Deserialize<T>(byte[] bytes);

        T DeserializeObject<T>(string value);
    }
}