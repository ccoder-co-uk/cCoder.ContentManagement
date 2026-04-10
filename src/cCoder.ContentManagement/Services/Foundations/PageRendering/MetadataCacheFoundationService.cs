using cCoder.ContentManagement.Rendering.Brokers;

namespace cCoder.ContentManagement.Rendering.Services.Foundations;

internal sealed class MetadataCacheFoundationService(IMetadataReaderBroker broker) : IMetadataCacheFoundationService
{
    public Func<string, string> Get(string culture) =>
        name => broker.GetMetadata(name, culture);
}
