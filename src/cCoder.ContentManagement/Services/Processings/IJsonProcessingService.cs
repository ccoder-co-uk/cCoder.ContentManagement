// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Services.Processings;

internal interface IJsonProcessingService
{
    T[] DeserializeItems<T>(string json);
}