// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Models.Serialization;

namespace cCoder.ContentManagement.Services.Foundations.Serialization;

internal partial class JsonService(
    IJsonBroker jsonBroker) : IJsonService
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

        object records = ParseRecordsPayload(json: jsonRecordsDocument.Json);

        return records is null || jsonBroker.IsJsonNull(value: records)
            ? null
            : new JsonRecordsDocument
            {
                Json = jsonBroker.Serialize(value: records),
                Records = GetRecordValues(records: records)
                    .Select(selector: CreateJsonObjectRecord)
                    .ToArray()
            };
    });

    private object ParseRecordsPayload(string json)
    {
        object parsedPayload = jsonBroker.ParseJsonElement(json: json);

        if (!jsonBroker.IsJsonObject(value: parsedPayload))
        {
            return parsedPayload;
        }

        return GetProperties(value: parsedPayload)
            .FirstOrDefault(predicate: property => string.Equals(
                a: property.Key,
                b: "value",
                comparisonType: StringComparison.OrdinalIgnoreCase))
            .Value ?? parsedPayload;
    }

    private IEnumerable<object> GetRecordValues(object records)
    {
        if (jsonBroker.IsJsonArray(value: records))
        {
            return GetItems(value: records);
        }

        return jsonBroker.IsJsonObject(value: records)
            ? [records]
            : [];
    }

    private JsonObjectRecord CreateJsonObjectRecord(object record)
    {
        KeyValuePair<string, object>[] properties = GetProperties(value: record)
            .Where(predicate: property => jsonBroker.IsJsonString(
                value: property.Value))
            .ToArray();

        return new JsonObjectRecord
        {
            RawText = jsonBroker.IsJsonElement(value: record)
                ? jsonBroker.GetJsonRawText(value: record)
                : jsonBroker.Serialize(value: record),
            StringValues = properties.ToDictionary(
                keySelector: property => property.Key,
                elementSelector: property => property.Value.ToString(),
                comparer: StringComparer.Ordinal),
            DateTimeOffsetValues = properties
                .Where(predicate: property => DateTimeOffset.TryParse(
                    input: property.Value.ToString(),
                    result: out _))
                .ToDictionary(
                    keySelector: property => property.Key,
                    elementSelector: property => DateTimeOffset.Parse(
                        input: property.Value.ToString()),
                    comparer: StringComparer.Ordinal)
        };
    }

    private IEnumerable<KeyValuePair<string, object>> GetProperties(
        object value) =>
        jsonBroker.IsJsonElement(value: value)
            ? jsonBroker.GetJsonElementProperties(value: value)
            : jsonBroker.GetJsonProperties(value: value);

    private IEnumerable<object> GetItems(object value) =>
        jsonBroker.IsJsonElement(value: value)
            ? jsonBroker.GetJsonElementItems(value: value)
            : jsonBroker.GetJsonItems(value: value);

}