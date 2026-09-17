// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace cCoder.ContentManagement.Dependencies.Serialization;

#pragma warning disable STXD003 // JsonConverter requires these framework override signatures.
internal sealed class DateTimeOffsetJsonConverterDependency
    : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TryGetDateTimeOffset(value: out DateTimeOffset value))
        {
            return value;
        }

        string date = reader.GetString();

        if (DateTimeOffset.TryParse(
            input: date,
            formatProvider: CultureInfo.InvariantCulture,
            styles: DateTimeStyles.AssumeUniversal,
            result: out value))
        {
            return value;
        }

        throw new JsonException(
            message: $"'{date}' is not a supported DateTimeOffset value.");
    }

    public override void Write(
        Utf8JsonWriter writer,
        DateTimeOffset value,
        JsonSerializerOptions options) =>
        writer.WriteStringValue(value: value);
}
#pragma warning restore STXD003