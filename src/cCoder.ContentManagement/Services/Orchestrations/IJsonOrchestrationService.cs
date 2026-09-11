// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Serialization;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal interface IJsonOrchestrationService
{
    string SerializeRuntimeValue(object value);

    JsonRecordsDocument ParseJsonRecordsDocument(
        JsonRecordsDocument jsonRecordsDocument);
}