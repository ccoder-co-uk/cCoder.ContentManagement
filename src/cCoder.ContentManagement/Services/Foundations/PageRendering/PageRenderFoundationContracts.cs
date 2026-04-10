
using cCoder.ContentManagement.Rendering.Models;

namespace cCoder.ContentManagement.Rendering.Services.Foundations;

internal interface IMetadataCacheFoundationService
{
    Func<string, string> Get(string culture);
}

internal interface ICommonObjectCacheFoundationService
{
    PageCacheSlice Get(PageRenderEngineRequest request);
}

internal interface IMarkupRenderFoundationService
{
    PageRenderResult Render(PageRenderSession session);
}
