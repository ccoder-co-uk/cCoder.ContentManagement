// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.PageRendering;
using cCoder.ContentManagement.Rendering.Services.Foundations;

namespace cCoder.ContentManagement.Rendering.Services.Orchestrations;

internal sealed partial class PageRenderExecutionOrchestrationService(
    IMetadataCacheService metadataCacheService,
    ICommonObjectCacheService commonObjectCacheService,
    IMarkupRenderService markupRenderService) : IPageRenderExecutionOrchestrationService
{
    public PageRenderSession RenderPageRenderSession(PageRenderSession session) =>
        TryCatch<PageRenderSession>(operation: () =>
    {
        ValidateRenderPageRenderSession(inputs: [session]);

        string culture = !string.IsNullOrWhiteSpace(value: session.Request.Culture)
            ? session.Request.Culture
            : session.App?.DefaultCulture ?? string.Empty;

        session.MetadataResolver = metadataCacheService.Get(culture: culture);

        PageCacheSlice pageCacheSlice = commonObjectCacheService.GetPageCacheSlice();
        session.CommonResourcesByLookup = pageCacheSlice.CommonResourcesByLookup;
        session.CommonComponentsByName = pageCacheSlice.CommonComponentsByName;
        session.CommonScriptsByName = pageCacheSlice.CommonScriptsByName;

        return markupRenderService.RenderPageRenderSession(session: session);

    });
}