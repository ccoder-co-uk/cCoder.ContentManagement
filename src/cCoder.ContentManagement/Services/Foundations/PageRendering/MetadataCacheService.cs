using cCoder.ContentManagement.Rendering.Brokers;

namespace cCoder.ContentManagement.Rendering.Services.Foundations;

internal sealed class MetadataCacheService(IMetadataReaderBroker broker) : IMetadataCacheService
{
    public Func<string, string> Get(string culture) =>
        name => broker.GetMetadata(name, culture);
}
