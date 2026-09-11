// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Models.Serialization;

namespace cCoder.ContentManagement.Services.Foundations.Serialization;

internal partial class JsonService(
    IJsonBroker jsonBroker,
    ISystemTextJsonBroker systemTextJsonBroker = null) : IJsonService
{
    public object ParseJson(string json) =>
        jsonBroker.ParseJson(json: json);

    public string Serialize(object value) =>
        jsonBroker.Serialize(value: value);

    public bool IsJsonObject(object value) =>
        jsonBroker.IsJsonObject(value: value);

    public bool IsJsonArray(object value) =>
        jsonBroker.IsJsonArray(value: value);

    public IEnumerable<KeyValuePair<string, object>> GetJsonProperties(object value) =>
        jsonBroker.GetJsonProperties(value: value);

    public IEnumerable<object> GetJsonItems(object value) =>
        jsonBroker.GetJsonItems(value: value);

    public void RemoveJsonProperty(object value, string propertyName) =>
        jsonBroker.RemoveJsonProperty(value: value, propertyName: propertyName);

    public T Deserialize<T>(string json) =>
        TryCatch<T>(operation: () =>
    {
        ValidateDeserialize(inputs: [json]);
        return jsonBroker.ParseJson<T>(json: json);
    });

    public JsonRecordsDocument ParseJsonRecordsDocument(
        JsonRecordsDocument jsonRecordsDocument) =>
        TryCatch<JsonRecordsDocument>(operation: () =>
    {
        ValidateDeserialize(inputs: [jsonRecordsDocument]);

        return systemTextJsonBroker.ParseRecords(json: jsonRecordsDocument.Json);
    });

}