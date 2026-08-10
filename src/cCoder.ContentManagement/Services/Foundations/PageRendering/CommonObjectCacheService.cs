// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Rendering.Brokers;
using cCoder.ContentManagement.Models.PageRendering;

namespace cCoder.ContentManagement.Rendering.Services.Foundations;

internal sealed partial class CommonObjectCacheService(ICommonObjectReaderBroker broker) : ICommonObjectCacheService
{
    public PageCacheSlice GetPageCacheSlice() =>
        TryCatch<PageCacheSlice>(operation: () =>
    {
        broker.EnsureAvailable();

        return new PageCacheSlice
        {
            CommonResourcesByLookup = broker.GetResourcesByLookup(),
            CommonComponentsByName = broker.GetComponentsByName(),
            CommonScriptsByName = broker.GetScriptsByName(),
            CommonStylesByName = broker.GetStylesByName()
        };

    });
}