// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Serialization;
using cCoder.ContentManagement.Services.Processings;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal sealed partial class RenderDataOrchestrationService(
    IJsonProcessingService jsonProcessingService,
    IPageRenderProcessingService pageRenderProcessingService,
    IHtmlRenderProcessingService htmlRenderProcessingService)
        : IRenderDataOrchestrationService
{
    public string SerializeRuntimeValue(object value) =>
        TryCatch<string>(operation: () =>
        {
            ValidateSerializeRuntimeValue(inputs: [value]);
            return pageRenderProcessingService.SerializeRuntimeValue(value: value);
        });

    public string HtmlEncode(string value) =>
        TryCatch<string>(operation: () =>
        {
            ValidateHtmlEncode(inputs: [value]);
            return htmlRenderProcessingService.HtmlEncode(value: value);
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