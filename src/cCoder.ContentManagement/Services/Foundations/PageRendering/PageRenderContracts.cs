
using cCoder.ContentManagement.Rendering.Models;

namespace cCoder.ContentManagement.Rendering.Services.Foundations;

internal interface IMetadataCacheService
{
    Func<string, string> Get(string culture);
}

internal interface ICommonObjectCacheService
{
    PageCacheSlice Get(PageRenderEngineRequest request);
}

internal interface IMarkupRenderService
{
    PageRenderResult Render(PageRenderSession session);
}
