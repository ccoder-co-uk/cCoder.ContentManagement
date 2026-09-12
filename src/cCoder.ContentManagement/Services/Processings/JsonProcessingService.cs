// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Foundations.Serialization;
using cCoder.ContentManagement.Models.Serialization;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class JsonProcessingService(
    IJsonService jsonService) : IJsonProcessingService
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

    public string Serialize(object value) =>
        TryCatch<string>(operation: () =>
    {
        ValidateSerialize(inputs: [value]);
        return jsonService.Serialize(value: value);
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

        object value = jsonService.ParseJson(json: json);

        RemovePropertiesRecursively(
            value: value,
            propertyNames: propertyNames);

        return jsonService.Serialize(value: value);
    });

    private void RemovePropertiesRecursively(
        object value,
        IReadOnlyCollection<string> propertyNames)
    {
        if (jsonService.IsJsonObject(value: value))
        {
            KeyValuePair<string, object>[] properties = jsonService
                .GetJsonProperties(value: value)
                .ToArray();

            foreach (KeyValuePair<string, object> property in properties)
            {
                if (propertyNames.Contains(
                    value: property.Key,
                    comparer: StringComparer.OrdinalIgnoreCase))
                {
                    jsonService.RemoveJsonProperty(
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
        else if (jsonService.IsJsonArray(value: value))
        {
            foreach (object item in jsonService.GetJsonItems(value: value))
            {
                RemovePropertiesRecursively(
                    value: item,
                    propertyNames: propertyNames);
            }
        }
    }
}