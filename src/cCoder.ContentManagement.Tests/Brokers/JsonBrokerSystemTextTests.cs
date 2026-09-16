// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Models.Serialization;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Services.Foundations.Serialization;
using FluentAssertions;
using System.Text.Json;
using Xunit;

namespace cCoder.ContentManagement.Tests.Brokers;

public sealed partial class JsonBrokerSystemTextTests
{
    [Fact]
    public void DeserializeSystemText_WhenPropertyCaseDiffers_IsCaseInsensitive()
    {
        // Given
        JsonBroker broker = new();

        // When
        TestPayload result = broker.ParseJson<TestPayload>(
            json: "{\"name\":\"Example\"}");

        // Then
        result.Name.Should()
            .Be(expected: "Example");
    }

    [Fact]
    public void SerializeSystemText_WhenMarkupIsPresent_PreservesSystemTextEscaping()
    {
        // Given
        JsonBroker broker = new();

        // When
        string result = broker.Serialize(
            value: new TestPayload { Name = "<script>" });

        // Then
        result.Should()
            .Be(expected: "{\"Name\":\"\\u003Cscript\\u003E\"}");
    }

    [Fact]
    public void ParseRecords_WhenPayloadIsSingleObject_ReturnsOneRecord()
    {
        // Given
        JsonBroker broker = new();
        JsonService service = new(jsonBroker: broker);
        JsonElement payload = ParseElement(json: "{\"name\":\"single\"}");

        // When
        JsonRecordsDocument result = service.ParseJsonRecordsDocument(
            jsonRecordsDocument: new JsonRecordsDocument
            {
                Json = payload.GetRawText()
            });

        // Then
        result.Records.Should()
            .ContainSingle();

        result.Records
            .Single()
            .StringValues["name"].Should()
            .Be(expected: "single");
    }

    [Fact]
    public void ParseRecords_WhenPayloadIsArray_ReturnsEveryRecord()
    {
        // Given
        JsonBroker broker = new();
        JsonService service = new(jsonBroker: broker);

        JsonElement payload = ParseElement(
            json: "[{\"name\":\"first\"},{\"name\":\"second\"}]");

        // When
        JsonRecordsDocument result = service.ParseJsonRecordsDocument(
            jsonRecordsDocument: new JsonRecordsDocument
            {
                Json = payload.GetRawText()
            });

        // Then
        result.Records
            .Select(selector: record => record.StringValues["name"])
            .Should()
            .Equal(elements: ["first", "second"]);
    }

    [Fact]
    public void ParseRecords_WhenPayloadHasValueWrapper_ReturnsWrappedRecords()
    {
        // Given
        JsonBroker broker = new();
        JsonService service = new(jsonBroker: broker);

        JsonElement payload = ParseElement(
            json: "{\"value\":[{\"name\":\"wrapped\"}]}");

        // When
        JsonRecordsDocument result = service.ParseJsonRecordsDocument(
            jsonRecordsDocument: new JsonRecordsDocument
            {
                Json = payload.GetRawText()
            });

        // Then
        result.Records.Should()
            .ContainSingle();

        result.Records
            .Single()
            .StringValues["name"].Should()
            .Be(expected: "wrapped");
    }

    [Fact]
    public void ParseRecords_WhenPayloadIsNull_ReturnsNull()
    {
        // Given
        JsonBroker broker = new();
        JsonService service = new(jsonBroker: broker);
        JsonElement payload = ParseElement(json: "null");

        // When
        JsonRecordsDocument result = service.ParseJsonRecordsDocument(
            jsonRecordsDocument: new JsonRecordsDocument
            {
                Json = payload.GetRawText()
            });

        // Then
        result.Should()
            .BeNull();
    }

    [Fact]
    public void Normalize_WhenValueIsNotJsonElement_PreservesOriginalValue()
    {
        // Given
        JsonBroker broker = new();
        TestPayload payload = new() { Name = "unchanged" };

        // When
        bool result = broker.IsJsonElement(value: payload);

        // Then
        result.Should()
            .BeFalse();
    }

    [Fact]
    public void Normalize_WhenValueIsJsonElement_ReturnsRawJsonWithoutLeakingElement()
    {
        // Given
        JsonBroker broker = new();
        JsonElement payload = ParseElement(json: "{\"name\":\"json\"}");

        // When
        bool isJsonElement = broker.IsJsonElement(value: payload);
        string result = broker.GetJsonRawText(value: payload);

        // Then
        isJsonElement.Should()
            .BeTrue();

        result.Should()
            .Be(expected: "{\"name\":\"json\"}");
    }

    [Fact]
    public void SerializeIgnoringReferences_WhenPackageComponentPayloadProvided_RoundTripsPayload()
    {
        // Given
        JsonBroker broker = new();

        Component[] payload =
        [
            new Component
            {
                Name = "Navigation",
                Key = "Core",
                Content = "<nav></nav>",
                Script = ""
            }
        ];

        // When
        string json = broker.SerializeIgnoringReferences(value: payload);
        Component[] result = broker.ParseJson<Component[]>(json: json);

        // Then
        result.Should()
            .ContainSingle()
            .Which.Name.Should()
            .Be(expected: "Navigation");

        result.Single()
            .Content.Should()
            .Be(expected: "<nav></nav>");
    }

    [Fact]
    public void RemoveJsonProperty_WhenNestedCachePayloadUsesDifferentCase_RemovesProperty()
    {
        // Given
        JsonBroker broker = new();
        object payload = broker.ParseJson(json: "{\"Name\":\"Navigation\",\"Children\":[]}");

        // When
        broker.RemoveJsonProperty(value: payload, propertyName: "name");
        string result = broker.Serialize(value: payload);

        // Then
        result.Should()
            .Be(expected: "{\"Children\":[]}");
    }

    private static JsonElement ParseElement(string json)
    {
        using JsonDocument document = JsonDocument.Parse(json: json);

        return document.RootElement.Clone();
    }

    private sealed class TestPayload
    {
        public string Name { get; init; }
    }
}