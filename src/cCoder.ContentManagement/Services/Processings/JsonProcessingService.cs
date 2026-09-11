// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Foundations.Serialization;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Models.Serialization;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class JsonProcessingService(
    IJsonService jsonService,
    IJsonBroker jsonBroker) : IJsonProcessingService
{
    public T[] DeserializeItems<T>(string json) =>
        TryCatch<T[]>(operation: () =>
    {
        ValidateDeserializeItems(inputs: [json]);

        return json.StartsWith(value: "{")
            ? [jsonService.Deserialize<T>(json: json)]
            : jsonService.Deserialize<T[]>(json: json);

    });

    public JsonRecordsDocument ParseJsonRecordsDocument(
        JsonRecordsDocument jsonRecordsDocument) =>
        TryCatch<JsonRecordsDocument>(operation: () =>
    {
        ValidateParseJsonRecordsDocument(inputs: [jsonRecordsDocument]);

        return jsonService.ParseJsonRecordsDocument(
            jsonRecordsDocument: jsonRecordsDocument);
    });

    public string RemovePropertiesRecursively(
        string json,
        IReadOnlyCollection<string> propertyNames) =>
        TryCatch<string>(operation: () =>
    {
        ValidateRemovePropertiesRecursively(inputs: [json, propertyNames]);

        if (string.IsNullOrWhiteSpace(value: json))
        {
            return json;
        }

        object value = jsonBroker.ParseJson(json: json);

        RemovePropertiesRecursively(
            value: value,
            propertyNames: propertyNames);

        return jsonBroker.Serialize(value: value);
    });

    private void RemovePropertiesRecursively(
        object value,
        IReadOnlyCollection<string> propertyNames)
    {
        if (jsonBroker.IsJsonObject(value: value))
        {
            KeyValuePair<string, object>[] properties = jsonBroker
                .GetJsonProperties(value: value)
                .ToArray();

            foreach (KeyValuePair<string, object> property in properties)
            {
                if (propertyNames.Contains(
                    value: property.Key,
                    comparer: StringComparer.OrdinalIgnoreCase))
                {
                    jsonBroker.RemoveJsonProperty(
                        value: value,
                        propertyName: property.Key);
                }
                else
                {
                    RemovePropertiesRecursively(
                        value: property.Value,
                        propertyNames: propertyNames);
                }
            }
        }
        else if (jsonBroker.IsJsonArray(value: value))
        {
            foreach (object item in jsonBroker.GetJsonItems(value: value))
            {
                RemovePropertiesRecursively(
                    value: item,
                    propertyNames: propertyNames);
            }
        }
    }
}