// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Brokers;

internal interface IJsonBroker
{
    object ParseJson(string json);

    object ParseJsonElement(string json);

    T ParseJson<T>(string json);

    string Serialize(object value);

    string SerializeIgnoringReferences(object value);

    bool IsJsonElement(object value);

    bool IsJsonNull(object value);

    bool IsJsonString(object value);

    string GetJsonRawText(object value);

    bool IsJsonObject(object value);

    bool IsJsonArray(object value);

    bool IsJsonValue(object value);

    IEnumerable<KeyValuePair<string, object>> GetJsonProperties(
        object value);

    IEnumerable<KeyValuePair<string, object>> GetJsonElementProperties(
        object value);

    IEnumerable<object> GetJsonItems(object value);

    IEnumerable<object> GetJsonElementItems(object value);

    void RemoveJsonProperty(object value, string propertyName);
}