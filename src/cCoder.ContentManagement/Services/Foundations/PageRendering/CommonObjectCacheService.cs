// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Rendering.Brokers;
using cCoder.ContentManagement.Rendering.Models;

namespace cCoder.ContentManagement.Rendering.Services.Foundations;

internal sealed partial class CommonObjectCacheService(ICommonObjectReaderBroker broker) : ICommonObjectCacheService
{
    public PageCacheSlice GetPageRenderEngineRequestPageCacheSlice(PageRenderEngineRequest request) =>
        TryCatch<PageCacheSlice>(operation: () =>
    {
        ValidatePageRenderEngineRequestPageCacheSliceOnGet(inputs: [request]);

        return new PageCacheSlice
        {
            CommonResourcesByLookup = broker.GetResourcesByLookup(),
            CommonComponentsByName = broker.GetComponentsByName(),
            CommonScriptsByName = broker.GetScriptsByName()
        };

    });
}