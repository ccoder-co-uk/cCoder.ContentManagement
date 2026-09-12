// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Services.Processings;

using cCoder.ContentManagement.Models.Serialization;

internal interface IJsonProcessingService
{
    T[] DeserializeItems<T>(string json);

    JsonRecordsDocument ParseJsonRecordsDocument(
        JsonRecordsDocument jsonRecordsDocument);

    string Serialize(object value);

    string RemovePropertiesRecursively(
        string json,
        IReadOnlyCollection<string> propertyNames);
}