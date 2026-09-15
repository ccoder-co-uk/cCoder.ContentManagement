// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Brokers;

using cCoder.ContentManagement.Models.Serialization;

internal interface IJsonBroker
{
    object ParseJson(string json);

    T ParseJson<T>(string json);

    string Serialize(object value);

    string SerializeIgnoringReferences(object value);

    JsonRecordsDocument ParseRecords(string json);

    JsonRecordsDocument ParseRecords(object payload);

    JsonValueDocument Normalize(object value);

    bool IsJsonObject(object value);

    bool IsJsonArray(object value);

    bool IsJsonValue(object value);

    IEnumerable<KeyValuePair<string, object>> GetJsonProperties(
        object value);

    IEnumerable<object> GetJsonItems(object value);

    void RemoveJsonProperty(object value, string propertyName);
}