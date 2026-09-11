// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;
using FluentAssertions;
using Xunit;

namespace cCoder.ContentManagement.Tests.Brokers;

public sealed class JsonBrokerSystemTextTests
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
        result.Name.Should().Be(expected: "Example");
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
        result.Should().Be(expected: "{\"Name\":\"\\u003Cscript\\u003E\"}");
    }

    private sealed class TestPayload
    {
        public string Name { get; init; }
    }
}