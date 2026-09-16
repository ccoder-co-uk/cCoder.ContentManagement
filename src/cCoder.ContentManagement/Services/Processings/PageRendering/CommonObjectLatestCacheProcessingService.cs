// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Caching;
using cCoder.ContentManagement.Rendering.Services.Foundations;
using cCoder.Data.Models;

namespace cCoder.ContentManagement.Rendering.Services.Processings;

internal sealed partial class CommonObjectLatestCacheProcessingService(
    ICommonObjectLatestCacheService commonObjectLatestCacheService)
        : ICommonObjectLatestCacheProcessingService
{
    public void RefreshCommonObjects(int changedCommonObjectCount) =>
        TryCatch(operation: () =>
        {
            ValidateCommonObjectsOnRefresh(inputs: [changedCommonObjectCount]);

            if (changedCommonObjectCount > 0)
            {
                _ = commonObjectLatestCacheService
                    .LoadCommonObjectCacheSnapshot();
            }
        });

    public CommonObjectCacheSnapshot LoadCommonObjectCacheSnapshot() =>
        TryCatch(operation: () => commonObjectLatestCacheService
            .LoadCommonObjectCacheSnapshot());

    public CommonObjectCacheSnapshot GetCommonObjectCacheSnapshot() =>
        TryCatch(operation: () => commonObjectLatestCacheService
            .GetCommonObjectCacheSnapshot());
}