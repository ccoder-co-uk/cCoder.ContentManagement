using cCoder.ContentManagement.Rendering.Brokers;
using cCoder.ContentManagement.Rendering.Models;

namespace cCoder.ContentManagement.Rendering.Services.Foundations;

internal sealed class CommonObjectCacheService(ICommonObjectReaderBroker broker) : ICommonObjectCacheService
{
    public PageCacheSlice Get(PageRenderEngineRequest request)
    {
        return new PageCacheSlice
        {
            CommonResourcesByLookup = broker.GetResourcesByLookup(),
            CommonComponentsByName = broker.GetComponentsByName(),
            CommonScriptsByName = broker.GetScriptsByName()
        };
    }
}
