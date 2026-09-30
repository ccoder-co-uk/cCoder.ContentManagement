// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Models.Serialization;
using cCoder.ContentManagement.Services.Foundations.Serialization;
using FluentAssertions;
using Xunit;

namespace cCoder.ContentManagement.Tests.Foundations.Serialization;

public sealed partial class JsonServiceRecordTests
{
    [Fact]
    public void ParseJson_WhenDateTimeOffsetHasNoOffset_AssumesUniversalTime()
    {
        // Given
        JsonBroker broker = new();

        // When
        DateTimeOffsetRecord result = broker.ParseJson<DateTimeOffsetRecord>(
            json: "{\"LastUpdated\":\"03/21/2022 12:37:55\"}");

        // Then
        result.LastUpdated.Should()
            .Be(expected: new DateTimeOffset(
                year: 2022,
                month: 3,
                day: 21,
                hour: 12,
                minute: 37,
                second: 55,
                millisecond: 0,
                offset: TimeSpan.Zero));
    }

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

    private sealed record DateTimeOffsetRecord(DateTimeOffset LastUpdated);
}