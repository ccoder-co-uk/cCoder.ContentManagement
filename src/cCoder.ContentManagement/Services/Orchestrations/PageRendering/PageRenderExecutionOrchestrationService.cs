// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Rendering.Models;
using cCoder.ContentManagement.Rendering.Services.Foundations;

namespace cCoder.ContentManagement.Rendering.Services.Orchestrations;

internal sealed class PageRenderExecutionOrchestrationService(
    IMetadataCacheService metadataCacheService,
    ICommonObjectCacheService commonObjectCacheService,
    IMarkupRenderService markupRenderService) : IPageRenderExecutionOrchestrationService
{
    public PageRenderResult RenderPageRenderSessionPageRenderResult(PageRenderSession session)
    {
        string culture = !string.IsNullOrWhiteSpace(value: session.Request.Culture)
            ? session.Request.Culture
            : session.App?.DefaultCulture ?? string.Empty;

        session.MetadataResolver = metadataCacheService.Get(culture: culture);

        PageCacheSlice pageCacheSlice = commonObjectCacheService.GetPageRenderEngineRequestPageCacheSlice(request: session.Request);
        session.CommonResourcesByLookup = pageCacheSlice.CommonResourcesByLookup;
        session.CommonComponentsByName = pageCacheSlice.CommonComponentsByName;
        session.CommonScriptsByName = pageCacheSlice.CommonScriptsByName;

        return markupRenderService.RenderPageRenderSessionPageRenderResult(session: session);
    }
}