// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Services.Foundations.Serialization;

using cCoder.ContentManagement.Models.Serialization;

internal interface IJsonService
{
    T Deserialize<T>(string json);

    JsonRecordsDocument ParseJsonRecordsDocument(
        JsonRecordsDocument jsonRecordsDocument);

    object ParseJson(string json);

    string Serialize(object value);

    bool IsJsonObject(object value);

    bool IsJsonArray(object value);

    IEnumerable<KeyValuePair<string, object>> GetJsonProperties(object value);

    IEnumerable<object> GetJsonItems(object value);

    void RemoveJsonProperty(object value, string propertyName);
}