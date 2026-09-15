// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Models.Serialization;
using cCoder.ContentManagement.Services.Foundations.Serialization;
using FluentAssertions;
using Xunit;

namespace cCoder.ContentManagement.Tests.Foundations.Serialization;

public sealed partial class JsonServiceRecordTests
{
    [Fact]
    public void ParseRecords_WhenPayloadIsAnArray_PreservesRawRecordTextAndValues()
    {
        // Given
        JsonService service = new(
            jsonBroker: new JsonBroker());

        // When
        JsonRecordsDocument document = service.ParseJsonRecordsDocument(
            jsonRecordsDocument: new JsonRecordsDocument
            {
                Json = "[ { \"Name\": \"First\", \"CreatedOn\": \"2026-09-11T12:00:00+00:00\" },{\"Name\":\"Second\"}]"
            });

        IReadOnlyCollection<JsonObjectRecord> results = document.Records;

        // Then
        results.Should()
            .HaveCount(expected: 2);

        results.First()
            .RawText.Should()
            .Be(expected: "{ \"Name\": \"First\", \"CreatedOn\": \"2026-09-11T12:00:00+00:00\" }");

        results.First()
            .StringValues["Name"].Should()
            .Be(expected: "First");

        results.First()
            .DateTimeOffsetValues["CreatedOn"].Should()
            .Be(expected: DateTimeOffset.Parse(input: "2026-09-11T12:00:00+00:00"));

        results.Last()
            .StringValues.Should()
            .NotContainKey(unexpected: "Missing");
    }
}