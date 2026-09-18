// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using cCoder.CodeAnalysis.Exposures;
using cCoder.ContentManagement.Dependencies.Serialization;

namespace cCoder.ContentManagement.Brokers;

internal sealed class JsonBroker : IJsonBroker, IUtilityBroker
{
    private static readonly JsonSerializerOptions IgnoreReferencesOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    };

    private static readonly JsonSerializerOptions ParseOptions = CreateParseOptions();

    public object ParseJson(string json) =>
        (object)JsonNode.Parse(json: json) ?? ParseNullElement();

    public object ParseJsonElement(string json) =>
        JsonSerializer.Deserialize<JsonElement>(json: json);

    public T ParseJson<T>(string json) =>
        JsonSerializer.Deserialize<T>(
            json: json,
            options: ParseOptions);

    public string Serialize(object value) =>
        JsonSerializer.Serialize(value: value);

    public string SerializeIgnoringReferences(object value) =>
        JsonSerializer.Serialize(value: value, options: IgnoreReferencesOptions);

    public string ComputeFingerprint(object value) =>
        Convert.ToHexString(
            inArray: SHA256.HashData(
                source: Encoding.UTF8.GetBytes(
                    s: JsonSerializer.Serialize(value: value))));

    public bool IsJsonElement(object value) =>
        value is JsonElement;

    public bool IsJsonNull(object value) =>
        value is JsonElement { ValueKind: JsonValueKind.Null }
        || value is JsonValue jsonValue && jsonValue.ToJsonString() == "null";

    public bool IsJsonString(object value) =>
        value is JsonElement { ValueKind: JsonValueKind.String }
        || value is JsonValue jsonValue
        && jsonValue.TryGetValue<string>(value: out _);

    public string GetJsonRawText(object value) =>
        ((JsonElement)value).GetRawText();

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

    public IEnumerable<KeyValuePair<string, object>> GetJsonElementProperties(
        object value) =>
        ((JsonElement)value)
            .EnumerateObject()
            .Select(selector: property =>
                new KeyValuePair<string, object>(
                    key: property.Name,
                    value: property.Value.Clone()));

    public IEnumerable<object> GetJsonItems(object value) =>
        ((JsonArray)value)
            .Select(selector: item => (object)item ?? ParseNullElement());

    public IEnumerable<object> GetJsonElementItems(object value) =>
        ((JsonElement)value)
            .EnumerateArray()
            .Select(selector: item => (object)item.Clone());

    public void RemoveJsonProperty(object value, string propertyName) =>
        ((JsonObject)value).Remove(
            propertyName: ((JsonObject)value)
                .Select(selector: property => property.Key)
                .FirstOrDefault(predicate: name => string.Equals(
                    a: name,
                    b: propertyName,
                    comparisonType: StringComparison.OrdinalIgnoreCase))
                ?? propertyName);

    private static JsonElement ParseNullElement()
    {
        using JsonDocument document = JsonDocument.Parse(json: "null");

        return document.RootElement.Clone();
    }

    private static JsonSerializerOptions CreateParseOptions()
    {
        JsonSerializerOptions options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        options.Converters.Add(
            item: new DateTimeOffsetJsonConverterDependency());

        return options;
    }
}