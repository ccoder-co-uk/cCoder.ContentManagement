// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;

namespace cCoder.ContentManagement.Services.Foundations.Serialization;

internal partial class JsonService(IJsonBroker jsonBroker) : IJsonService
{
    public T Deserialize<T>(string json) =>
        TryCatch<T>(operation: () =>
    {
        ValidateDeserialize(inputs: [json]);
        return jsonBroker.ParseJson<T>(json: json);
    });
}