// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Services.Processings;

public interface IJsonProcessingService
{
    T[] DeserializeItems<T>(string json);
}