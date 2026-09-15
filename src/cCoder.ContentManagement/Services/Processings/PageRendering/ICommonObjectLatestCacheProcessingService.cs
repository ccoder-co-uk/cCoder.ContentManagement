// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;

namespace cCoder.ContentManagement.Rendering.Services.Processings;

internal interface ICommonObjectLatestCacheProcessingService
{
    void RefreshCommonObjects(int changedCommonObjectCount);

    IEnumerable<CommonObject> GetLatestCommonObjects();
}
