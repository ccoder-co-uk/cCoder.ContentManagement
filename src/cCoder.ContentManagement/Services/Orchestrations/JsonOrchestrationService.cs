// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Serialization;
using cCoder.ContentManagement.Services.Processings;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal sealed partial class JsonOrchestrationService(
    IJsonProcessingService jsonProcessingService,
    IPageRenderProcessingService pageRenderProcessingService)
        : IJsonOrchestrationService
{
    public string SerializeRuntimeValue(object value) =>
        TryCatch<string>(operation: () =>
    {
        ValidateSerializeRuntimeValue(inputs: [value]);
        return pageRenderProcessingService.SerializeRuntimeValue(value: value);
    });

    public JsonRecordsDocument ParseJsonRecordsDocument(
        JsonRecordsDocument jsonRecordsDocument) =>
        TryCatch<JsonRecordsDocument>(operation: () =>
    {
        ValidateParseJsonRecordsDocument(inputs: [jsonRecordsDocument]);

        return jsonProcessingService.ParseJsonRecordsDocument(
            jsonRecordsDocument: jsonRecordsDocument);
    });
}