// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Brokers;

public interface IJsonBroker
{
    object ParseJson(string json);

    T ParseJson<T>(string json);

    string Serialize(object value);

    string SerializeIgnoringReferences(object value);

    bool IsJsonObject(object value);

    bool IsJsonArray(object value);

    bool IsJsonValue(object value);

    IEnumerable<KeyValuePair<string, object>> GetJsonProperties(
        object value);

    IEnumerable<object> GetJsonItems(object value);

    void RemoveJsonProperty(object value, string propertyName);
}