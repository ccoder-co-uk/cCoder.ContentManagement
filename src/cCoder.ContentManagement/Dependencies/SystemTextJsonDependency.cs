// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using cCoder.ContentManagement.Models.Serialization;

namespace cCoder.ContentManagement.Dependencies;

internal sealed class SystemTextJsonDependency
{
    private static readonly JsonSerializerOptions IgnoreReferencesOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    };

    public object ParseJson(string json) =>
        (object)JsonNode.Parse(json: json) ?? ParseNullElement();

    public T Deserialize<T>(string json) =>
        JsonSerializer.Deserialize<T>(
            json: json,
            options: new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

    public string Serialize(object value) =>
        JsonSerializer.Serialize(value: value);

    public string SerializeIgnoringReferences(object value) =>
        JsonSerializer.Serialize(value: value, options: IgnoreReferencesOptions);

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

    public bool IsJsonObject(object value) =>
        value is JsonObject ||
        value is JsonElement { ValueKind: JsonValueKind.Object };

    public bool IsJsonArray(object value) =>
        value is JsonArray ||
        value is JsonElement { ValueKind: JsonValueKind.Array };

    public bool IsJsonValue(object value) =>
        value is JsonValue ||
        value is JsonElement element &&
        element.ValueKind is not JsonValueKind.Object and not JsonValueKind.Array;

    public IEnumerable<KeyValuePair<string, object>> GetJsonProperties(
        object value) =>
        ((JsonObject)value)
            .Select(selector: property =>
                new KeyValuePair<string, object>(
                    key: property.Key,
                    value: (object)property.Value ?? ParseNullElement()));

    public IEnumerable<object> GetJsonItems(object value) =>
        ((JsonArray)value)
            .Select(selector: item => (object)item ?? ParseNullElement());

    public void RemoveJsonProperty(object value, string propertyName)
    {
        JsonObject jsonObject = (JsonObject)value;

        string matchingPropertyName = jsonObject
            .Select(selector: property => property.Key)
            .FirstOrDefault(predicate: name => string.Equals(
                a: name,
                b: propertyName,
                comparisonType: StringComparison.OrdinalIgnoreCase));

        if (matchingPropertyName is not null)
        {
            jsonObject.Remove(propertyName: matchingPropertyName);
        }
    }

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

    private static JsonElement ParseNullElement()
    {
        using JsonDocument document = JsonDocument.Parse(json: "null");

        return document.RootElement.Clone();
    }
}