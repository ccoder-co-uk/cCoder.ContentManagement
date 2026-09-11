// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Models.Serialization;
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
        SystemTextJsonBroker broker = new();

        // When
        TestPayload result = broker.Deserialize<TestPayload>(
            json: "{\"name\":\"Example\"}");

        // Then
        result.Name.Should()
            .Be(expected: "Example");
    }

    [Fact]
    public void SerializeSystemText_WhenMarkupIsPresent_PreservesSystemTextEscaping()
    {
        // Given
        SystemTextJsonBroker broker = new();

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
        SystemTextJsonBroker broker = new();
        JsonElement payload = ParseElement(json: "{\"name\":\"single\"}");

        // When
        JsonRecordsDocument result = broker.ParseRecords(payload: payload);

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
        SystemTextJsonBroker broker = new();

        JsonElement payload = ParseElement(
            json: "[{\"name\":\"first\"},{\"name\":\"second\"}]");

        // When
        JsonRecordsDocument result = broker.ParseRecords(payload: payload);

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
        SystemTextJsonBroker broker = new();

        JsonElement payload = ParseElement(
            json: "{\"value\":[{\"name\":\"wrapped\"}]}");

        // When
        JsonRecordsDocument result = broker.ParseRecords(payload: payload);

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
        SystemTextJsonBroker broker = new();
        JsonElement payload = ParseElement(json: "null");

        // When
        JsonRecordsDocument result = broker.ParseRecords(payload: payload);

        // Then
        result.Should()
            .BeNull();
    }

    [Fact]
    public void Normalize_WhenValueIsNotJsonElement_PreservesOriginalValue()
    {
        // Given
        SystemTextJsonBroker broker = new();
        TestPayload payload = new() { Name = "unchanged" };

        // When
        JsonValueDocument result = broker.Normalize(value: payload);

        // Then
        result.IsRawJson.Should()
            .BeFalse();

        result.Value.Should()
            .BeSameAs(expected: payload);
    }

    [Fact]
    public void Normalize_WhenValueIsJsonElement_ReturnsRawJsonWithoutLeakingElement()
    {
        // Given
        SystemTextJsonBroker broker = new();
        JsonElement payload = ParseElement(json: "{\"name\":\"json\"}");

        // When
        JsonValueDocument result = broker.Normalize(value: payload);

        // Then
        result.IsRawJson.Should()
            .BeTrue();

        result.RawJson.Should()
            .Be(expected: "{\"name\":\"json\"}");

        result.Value.Should()
            .BeNull();
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