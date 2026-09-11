// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text.Json;

namespace cCoder.ContentManagement.Brokers;

internal sealed class SystemTextJsonBroker : ISystemTextJsonBroker
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

    public JsonDocument Parse(string json) =>
        JsonDocument.Parse(json: json);
}