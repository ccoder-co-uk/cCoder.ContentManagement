// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Caching;
using cCoder.ContentManagement.Models.PageRendering;

namespace cCoder.ContentManagement.Rendering.Services.Processings;

internal interface ICommonObjectCacheProcessingService
{
    bool IsAvailable();

    T[] GetAll<T>();

    T Get<T>(string key);

    void Set(string key, object item);

    void SetCommonObjectCacheSnapshot(
        CommonObjectCacheSnapshot commonObjectCacheSnapshot);

    CommonObjectCacheSnapshot GetCommonObjectCacheSnapshot();
}

internal interface ICommonObjectRenderCacheProcessingService
{
    RenderSession PrepareRenderSession(RenderSession session);
}