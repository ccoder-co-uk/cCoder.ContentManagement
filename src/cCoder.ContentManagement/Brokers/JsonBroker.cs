using Newtonsoft.Json;

namespace cCoder.ContentManagement.Brokers;

public class JsonBroker : IJsonBroker
{
    public object ParseJson(string json)
    {
        return JsonConvert.DeserializeObject(json);
    }

    public T ParseJson<T>(string json)
    {
        return JsonConvert.DeserializeObject<T>(json);
    }

    public string Serialize(object value)
    {
        return JsonConvert.SerializeObject(value);
    }
}
