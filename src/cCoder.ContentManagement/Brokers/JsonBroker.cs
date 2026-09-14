// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;
using cCoder.ContentManagement.Models.Serialization;

namespace cCoder.ContentManagement.Brokers;

internal sealed class JsonBroker(
    SystemTextJsonDependency dependency = null) : IJsonBroker
{
    private readonly SystemTextJsonDependency dependency =
        dependency ?? new SystemTextJsonDependency();

    public object ParseJson(string json) =>
        dependency.ParseJson(json: json);

    public T ParseJson<T>(string json) =>
        dependency.Deserialize<T>(json: json);

    public string Serialize(object value) =>
        dependency.Serialize(value: value);

    public string SerializeIgnoringReferences(object value) =>
        dependency.SerializeIgnoringReferences(value: value);

    public JsonRecordsDocument ParseRecords(string json) =>
        dependency.ParseRecords(json: json);

    public JsonRecordsDocument ParseRecords(object payload) =>
        dependency.ParseRecords(payload: payload);

    public JsonValueDocument Normalize(object value) =>
        dependency.Normalize(value: value);

    public bool IsJsonObject(object value) =>
        dependency.IsJsonObject(value: value);

    public bool IsJsonArray(object value) =>
        dependency.IsJsonArray(value: value);

    public bool IsJsonValue(object value) =>
        dependency.IsJsonValue(value: value);

    public IEnumerable<KeyValuePair<string, object>> GetJsonProperties(
        object value) =>
        dependency.GetJsonProperties(value: value);

    public IEnumerable<object> GetJsonItems(object value) =>
        dependency.GetJsonItems(value: value);

    public void RemoveJsonProperty(object value, string propertyName) =>
        dependency.RemoveJsonProperty(value: value, propertyName: propertyName);
}