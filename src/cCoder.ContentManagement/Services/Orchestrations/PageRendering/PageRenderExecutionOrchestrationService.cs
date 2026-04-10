using cCoder.ContentManagement.Rendering.Models;
using cCoder.ContentManagement.Rendering.Services.Foundations;

namespace cCoder.ContentManagement.Rendering.Services.Orchestrations;

internal sealed class PageRenderExecutionOrchestrationService(
    IMetadataCacheFoundationService metadataCacheFoundationService,
    ICommonObjectCacheFoundationService commonObjectCacheFoundationService,
    IMarkupRenderFoundationService markupRenderFoundationService) : IPageRenderExecutionOrchestrationService
{
    public PageRenderResult Render(PageRenderSession session)
    {
        string culture = !string.IsNullOrWhiteSpace(session.Request.Culture)
            ? session.Request.Culture
            : session.App?.DefaultCulture ?? string.Empty;

        session.MetadataResolver = metadataCacheFoundationService.Get(culture);

        PageCacheSlice pageCacheSlice = commonObjectCacheFoundationService.Get(session.Request);
        session.CommonResourcesByLookup = pageCacheSlice.CommonResourcesByLookup;
        session.CommonComponentsByName = pageCacheSlice.CommonComponentsByName;
        session.CommonScriptsByName = pageCacheSlice.CommonScriptsByName;

        return markupRenderFoundationService.Render(session);
    }
}
