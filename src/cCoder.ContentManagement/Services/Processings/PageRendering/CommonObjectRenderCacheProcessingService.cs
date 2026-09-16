// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.PageRendering;
using cCoder.ContentManagement.Rendering.Services.Foundations;

namespace cCoder.ContentManagement.Rendering.Services.Processings;

internal sealed partial class CommonObjectRenderCacheProcessingService(
    ICommonObjectRenderCacheService commonObjectRenderCacheService)
        : ICommonObjectRenderCacheProcessingService
{
    public RenderSession PrepareRenderSession(RenderSession renderSession) =>
        TryCatch(operation: () =>
        {
            ValidatePrepareRenderSession(inputs: [renderSession]);

            PageCacheSlice pageCacheSlice = commonObjectRenderCacheService
                .GetPageCacheSlice();

            renderSession.CommonResourcesByLookup =
                pageCacheSlice.CommonResourcesByLookup;

            renderSession.CommonComponentsByName =
                pageCacheSlice.CommonComponentsByName;

            renderSession.CommonScriptsByName =
                pageCacheSlice.CommonScriptsByName;

            renderSession.CommonStylesByName =
                pageCacheSlice.CommonStylesByName;

            return renderSession;
        });
}