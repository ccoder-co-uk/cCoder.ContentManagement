// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Rendering.Brokers;

namespace cCoder.ContentManagement.Rendering.Services.Foundations;

internal sealed partial class MetadataCacheService(IMetadataReaderBroker broker) : IMetadataCacheService
{
    public Func<string, string> Get(string culture) =>
        TryCatch<Func<string, string>>(operation: () =>
    {
        ValidateGet(inputs: [culture]);
        return name => broker.GetMetadata(name: name, culture: culture);
    });
}