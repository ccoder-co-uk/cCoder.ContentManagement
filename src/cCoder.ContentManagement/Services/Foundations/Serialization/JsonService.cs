// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
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

    public IReadOnlyCollection<KeyValuePair<string, object>> GetJsonProperties(object value) =>
        TryCatch<IReadOnlyCollection<KeyValuePair<string, object>>>(operation: () =>
    {
        ValidateJsonPropertiesOnGet(inputs: [value]);

        List<KeyValuePair<string, object>> properties = [];

        foreach (KeyValuePair<string, object> property in jsonBroker.GetJsonProperties(value: value))
        {
            properties.Add(item: property);
        }

        return properties;
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

        if (records is null || jsonBroker.IsJsonNull(value: records))
        {
            return null;
        }

        List<JsonObjectRecord> recordValues = [];

        foreach (object record in GetRecordValues(records: records))
        {
            recordValues.Add(item: CreateJsonObjectRecord(record: record));
        }

        return new JsonRecordsDocument
        {
            Json = jsonBroker.Serialize(value: records),
            Records = [.. recordValues]
        };
    });

    private object ParseRecordsPayload(string json)
    {
        object parsedPayload = jsonBroker.ParseJsonElement(json: json);

        if (!jsonBroker.IsJsonObject(value: parsedPayload))
        {
            return parsedPayload;
        }

        foreach (KeyValuePair<string, object> property in
            GetProperties(value: parsedPayload))
        {
            if (string.Equals(
                a: property.Key,
                b: "value",
                comparisonType: StringComparison.OrdinalIgnoreCase))
            {
                return property.Value ?? parsedPayload;
            }
        }

        return parsedPayload;
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
        Dictionary<string, string> stringValues = new(StringComparer.Ordinal);
        Dictionary<string, DateTimeOffset> dateTimeOffsetValues = new(StringComparer.Ordinal);

        foreach (KeyValuePair<string, object> property in GetProperties(value: record))
        {
            if (!jsonBroker.IsJsonString(value: property.Value))
            {
                continue;
            }

            string value = jsonBroker.GetJsonString(value: property.Value);

            stringValues[property.Key] = value;

            if (DateTimeOffset.TryParse(
                input: value,
                result: out DateTimeOffset dateTimeOffset))
            {
                dateTimeOffsetValues[property.Key] = dateTimeOffset;
            }
        }

        return new JsonObjectRecord
        {
            RawText = jsonBroker.IsJsonElement(value: record)
                ? jsonBroker.GetJsonRawText(value: record)
                : jsonBroker.Serialize(value: record),
            StringValues = stringValues,
            DateTimeOffsetValues = dateTimeOffsetValues
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