// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Models.Serialization;
using System.Text.Json;

namespace cCoder.ContentManagement.Services.Foundations.Serialization;

internal partial class JsonService(
    IJsonBroker jsonBroker,
    ISystemTextJsonBroker systemTextJsonBroker = null) : IJsonService
{
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

        using JsonDocument jsonDocument = systemTextJsonBroker.Parse(
            json: jsonRecordsDocument.Json);

        IEnumerable<JsonElement> records =
            jsonDocument.RootElement.ValueKind == JsonValueKind.Array
                ? jsonDocument.RootElement.EnumerateArray()
                : [jsonDocument.RootElement];

        return new JsonRecordsDocument
        {
            Json = jsonRecordsDocument.Json,
            Records = records
                .Select(selector: static record => new JsonObjectRecord
                {
                    RawText = record.GetRawText(),
                    StringValues = record.EnumerateObject()
                        .Where(predicate: static property =>
                            property.Value.ValueKind == JsonValueKind.String)
                        .ToDictionary(
                            keySelector: static property => property.Name,
                            elementSelector: static property =>
                                property.Value.GetString(),
                            comparer: StringComparer.Ordinal),
                    DateTimeOffsetValues = record.EnumerateObject()
                        .Where(predicate: static property =>
                            property.Value.ValueKind == JsonValueKind.String
                            && property.Value.TryGetDateTimeOffset(value: out _))
                        .ToDictionary(
                            keySelector: static property => property.Name,
                            elementSelector: static property =>
                                property.Value.GetDateTimeOffset(),
                            comparer: StringComparer.Ordinal)
                })
                .ToArray()
        };
    });

}