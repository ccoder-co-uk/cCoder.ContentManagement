namespace cCoder.ContentManagement.Brokers;

public interface IJsonBroker
{
    object ParseJson(string json);

    T ParseJson<T>(string json);

    string Serialize(object value);
}
