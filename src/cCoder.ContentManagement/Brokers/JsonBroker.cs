// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Newtonsoft.Json;

namespace cCoder.ContentManagement.Brokers;

internal sealed class JsonBroker : IJsonBroker
{
    public object ParseJson(string json) =>
        JsonConvert.DeserializeObject(value: json);

    public T ParseJson<T>(string json)
    {
        return JsonConvert.DeserializeObject<T>(value: json);
    }

    public string Serialize(object value) =>
        JsonConvert.SerializeObject(value: value);
}