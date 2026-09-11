// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text.Json;
using cCoder.ContentManagement.Models.Serialization;
using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Brokers;

internal sealed class SystemTextJsonBroker(
    SystemTextJsonDependency dependency = null) : ISystemTextJsonBroker
{
    private readonly SystemTextJsonDependency dependency =
        dependency ?? new SystemTextJsonDependency();

    public T Deserialize<T>(string json) =>
        dependency.Deserialize<T>(json: json);

    public string Serialize(object value) =>
        dependency.Serialize(value: value);

    public JsonRecordsDocument ParseRecords(string json) =>
        dependency.ParseRecords(json: json);

    public JsonRecordsDocument ParseRecords(object payload) =>
        dependency.ParseRecords(payload: payload);

    public JsonValueDocument Normalize(object value) =>
        dependency.Normalize(value: value);
}