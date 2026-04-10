using cCoder.ContentManagement.Exposures.Caching;

namespace cCoder.ContentManagement.Rendering.Brokers;

internal sealed class MetadataReaderBroker(IMetadataCache metadataCache) : IMetadataReaderBroker
{
    public string GetMetadata(string name, string culture) =>
        metadataCache.Get(name, culture) ?? string.Empty;
}
