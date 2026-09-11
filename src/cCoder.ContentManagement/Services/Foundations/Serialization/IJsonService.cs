// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Services.Foundations.Serialization;

using cCoder.ContentManagement.Models.Serialization;

internal interface IJsonService
{
    T Deserialize<T>(string json);

    JsonRecordsDocument ParseJsonRecordsDocument(
        JsonRecordsDocument jsonRecordsDocument);
}