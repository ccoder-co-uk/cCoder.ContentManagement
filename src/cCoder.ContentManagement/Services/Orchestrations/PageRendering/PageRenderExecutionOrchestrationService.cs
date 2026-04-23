using cCoder.ContentManagement.Rendering.Models;
using cCoder.ContentManagement.Rendering.Services.Foundations;

namespace cCoder.ContentManagement.Rendering.Services.Orchestrations;

internal sealed class PageRenderExecutionOrchestrationService(
    IMetadataCacheService metadataCacheService,
    ICommonObjectCacheService commonObjectCacheService,
    IMarkupRenderService markupRenderService) : IPageRenderExecutionOrchestrationService
{
    public PageRenderResult Render(PageRenderSession session)
    {
        string culture = !string.IsNullOrWhiteSpace(session.Request.Culture)
            ? session.Request.Culture
            : session.App?.DefaultCulture ?? string.Empty;

        session.MetadataResolver = metadataCacheService.Get(culture);

        PageCacheSlice pageCacheSlice = commonObjectCacheService.Get(session.Request);
        session.CommonResourcesByLookup = pageCacheSlice.CommonResourcesByLookup;
        session.CommonComponentsByName = pageCacheSlice.CommonComponentsByName;
        session.CommonScriptsByName = pageCacheSlice.CommonScriptsByName;

        return markupRenderService.Render(session);
    }
}
