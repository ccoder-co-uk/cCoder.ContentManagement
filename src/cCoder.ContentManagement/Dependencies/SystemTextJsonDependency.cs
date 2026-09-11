// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text.Json;
using cCoder.ContentManagement.Models.Serialization;

namespace cCoder.ContentManagement.Dependencies;

internal sealed class SystemTextJsonDependency
{
    public T Deserialize<T>(string json) =>
        JsonSerializer.Deserialize<T>(
            json: json,
            options: new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

    public string Serialize(object value) =>
        JsonSerializer.Serialize(value: value);

    public JsonRecordsDocument ParseRecords(string json) =>
        ParseRecordsDocument(json: json);

    public JsonRecordsDocument ParseRecords(object payload)
    {
        if (payload is null)
        {
            return null;
        }

        string json = payload is JsonElement element
            ? element.GetRawText()
            : Serialize(value: payload);

        return ParseRecordsDocument(json: json);
    }

    public JsonValueDocument Normalize(object value) =>
        value is JsonElement element
            ? new JsonValueDocument
            {
                IsRawJson = true,
                RawJson = element.GetRawText()
            }
            : new JsonValueDocument
            {
                Value = value
            };

    private static JsonRecordsDocument ParseRecordsDocument(string json)
    {
        using JsonDocument document = JsonDocument.Parse(json: json);
        JsonElement root = document.RootElement;

        if (root.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        if (root.ValueKind == JsonValueKind.Object &&
            root.TryGetProperty(propertyName: "value", value: out JsonElement value))
        {
            root = value;
        }

        if (root.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        IEnumerable<JsonElement> records = root.ValueKind switch
        {
            JsonValueKind.Array => root.EnumerateArray(),
            JsonValueKind.Object => [root],
            var unsupportedKind => []
        };

        return new JsonRecordsDocument
        {
            Json = root.GetRawText(),
            Records = records
                .Select(selector: CreateRecord)
                .ToArray()
        };
    }

    private static JsonObjectRecord CreateRecord(JsonElement record) =>
        new()
        {
            RawText = record.GetRawText(),
            StringValues = record.ValueKind == JsonValueKind.Object
                ? record.EnumerateObject()
                    .Where(predicate: static property =>
                        property.Value.ValueKind == JsonValueKind.String)
                    .ToDictionary(
                        keySelector: static property => property.Name,
                        elementSelector: static property => property.Value.GetString(),
                        comparer: StringComparer.Ordinal)
                : new Dictionary<string, string>(),
            DateTimeOffsetValues = record.ValueKind == JsonValueKind.Object
                ? record.EnumerateObject()
                    .Where(predicate: static property =>
                        property.Value.ValueKind == JsonValueKind.String &&
                        property.Value.TryGetDateTimeOffset(value: out _))
                    .ToDictionary(
                        keySelector: static property => property.Name,
                        elementSelector: static property => property.Value.GetDateTimeOffset(),
                        comparer: StringComparer.Ordinal)
                : new Dictionary<string, DateTimeOffset>()
        };
}