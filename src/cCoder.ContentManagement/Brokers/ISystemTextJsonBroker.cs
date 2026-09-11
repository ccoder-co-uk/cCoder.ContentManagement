// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text.Json;

namespace cCoder.ContentManagement.Brokers;

internal interface ISystemTextJsonBroker
{
    T Deserialize<T>(string json);

    string Serialize(object value);

    JsonDocument Parse(string json);
}