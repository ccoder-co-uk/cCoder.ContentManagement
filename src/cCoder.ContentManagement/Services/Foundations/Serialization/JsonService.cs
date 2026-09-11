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
        TryCatch<object>(operation: () =>
    {
        ValidateJsonOnParse(inputs: [json]);
        return jsonBroker.ParseJson(json: json);
    });

    public string Serialize(object value) =>
        TryCatch<string>(operation: () =>
    {
        ValidateValueOnSerialize(inputs: [value]);
        return jsonBroker.Serialize(value: value);
    });

    public bool IsJsonObject(object value) =>
        TryCatch<bool>(operation: () =>
    {
        ValidateJsonObjectOnCheck(inputs: [value]);
        return jsonBroker.IsJsonObject(value: value);
    });

    public bool IsJsonArray(object value) =>
        TryCatch<bool>(operation: () =>
    {
        ValidateJsonArrayOnCheck(inputs: [value]);
        return jsonBroker.IsJsonArray(value: value);
    });

    public IEnumerable<KeyValuePair<string, object>> GetJsonProperties(object value) =>
        TryCatch<IEnumerable<KeyValuePair<string, object>>>(operation: () =>
    {
        ValidateJsonPropertiesOnGet(inputs: [value]);
        return jsonBroker.GetJsonProperties(value: value);
    });

    public IEnumerable<object> GetJsonItems(object value) =>
        TryCatch<IEnumerable<object>>(operation: () =>
    {
        ValidateJsonItemsOnGet(inputs: [value]);
        return jsonBroker.GetJsonItems(value: value);
    });

    public void RemoveJsonProperty(object value, string propertyName) =>
        TryCatch<object>(operation: () =>
    {
        ValidateJsonPropertyOnRemove(inputs: [value, propertyName]);
        jsonBroker.RemoveJsonProperty(value: value, propertyName: propertyName);
        return null;
    });

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
        ValidateJsonRecordsDocumentOnParse(inputs: [jsonRecordsDocument]);

        return systemTextJsonBroker.ParseRecords(json: jsonRecordsDocument.Json);
    });

}