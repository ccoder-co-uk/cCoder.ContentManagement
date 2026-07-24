// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Foundations.Serialization;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class JsonProcessingService(
    IJsonService jsonService) : IJsonProcessingService
{
    public T[] DeserializeItems<T>(string json) =>
        TryCatch<T[]>(operation: () =>
    {
        ValidateDeserializeItems(inputs: [json]);

        return json.StartsWith(value: "{")
            ? [jsonService.Deserialize<T>(json: json)]
            : jsonService.Deserialize<T[]>(json: json);

    });
}