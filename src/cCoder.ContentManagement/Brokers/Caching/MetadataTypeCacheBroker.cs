// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Exposures;

namespace cCoder.ContentManagement.Brokers.Caching;

internal sealed class MetadataTypeCacheBroker(
    IMetadataTypeCache metadataTypeCache) : IMetadataTypeCacheBroker
{
    public IEnumerable<string> GetAll() =>
        metadataTypeCache.GetAll();
}