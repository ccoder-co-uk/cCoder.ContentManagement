// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.ContentManagement.Models.Caching;

namespace cCoder.ContentManagement.Rendering.Services.Processings;

internal interface ICommonObjectLatestCacheProcessingService
{
    void RefreshCommonObjects(int changedCommonObjectCount);

    CommonObjectCacheSnapshot LoadCommonObjectCacheSnapshot();

    CommonObjectCacheSnapshot GetCommonObjectCacheSnapshot();
}