// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Serialization;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal interface IRenderDataOrchestrationService
{
    string SerializeRuntimeValue(object value);

    string HtmlEncode(string value);

    JsonRecordsDocument ParseJsonRecordsDocument(
        JsonRecordsDocument jsonRecordsDocument);
}