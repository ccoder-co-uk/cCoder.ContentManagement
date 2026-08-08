// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.PageRendering;
using cCoder.ContentManagement.Rendering.Services.Foundations;

namespace cCoder.ContentManagement.Rendering.Services.Processings;

internal sealed partial class CommonObjectCacheProcessingService(
    ICommonObjectCacheService commonObjectCacheService) : ICommonObjectCacheProcessingService
{
    public RenderSession PrepareRenderSession(RenderSession session) =>
        TryCatch(operation: () =>
    {
        ValidatePrepareRenderSession(inputs: [session]);

        PageCacheSlice pageCacheSlice = commonObjectCacheService.GetPageCacheSlice();
        session.CommonResourcesByLookup = pageCacheSlice.CommonResourcesByLookup;
        session.CommonComponentsByName = pageCacheSlice.CommonComponentsByName;
        session.CommonScriptsByName = pageCacheSlice.CommonScriptsByName;
        session.CommonStylesByName = pageCacheSlice.CommonStylesByName;

        return session;
    });
}