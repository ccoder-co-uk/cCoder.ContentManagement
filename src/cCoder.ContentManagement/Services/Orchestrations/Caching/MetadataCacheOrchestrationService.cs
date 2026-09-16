// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Caching;
using cCoder.ContentManagement.Rendering.Services.Processings;

namespace cCoder.ContentManagement.Services.Orchestrations.Caching;

internal sealed partial class MetadataCacheOrchestrationService(
    IMetadataCacheProcessingService metadataCacheProcessingService,
    IMetadataCacheSourceProcessingService metadataCacheSourceProcessingService)
        : IMetadataCacheOrchestrationService
{
    public string Get(string key, string culture) =>
        TryCatch(operation: () =>
        {
            ValidateMetadataOnGet(inputs: [key, culture]);
            MetadataCacheSnapshot snapshot = EnsureSynchronized();

            return snapshot.Serialized.TryGetValue(
                key: culture,
                value: out IDictionary<string, string> values)
                && values.TryGetValue(key: key, value: out string value)
                    ? value
                    : string.Empty;
        });

    public string GetAll(string culture) =>
        TryCatch(operation: () =>
        {
            ValidateAllOnGet(inputs: [culture]);
            MetadataCacheSnapshot snapshot = EnsureSynchronized();

            return snapshot.AllJson.TryGetValue(
                key: culture,
                value: out string value)
                    ? value
                    : "[]";
        });

    public void Rebuild() =>
        TryCatch(operation: () =>
        {
            _ = RebuildCore();
        });

    public string ToJson(string culture) =>
        TryCatch(operation: () =>
        {
            ValidateMetadataOnToJson(inputs: [culture]);
            MetadataCacheSnapshot snapshot = EnsureSynchronized();

            return snapshot.DictionaryJson.TryGetValue(
                key: culture,
                value: out string value)
                    ? value
                    : "{}";
        });

    private MetadataCacheSnapshot EnsureSynchronized()
    {
        MetadataCacheSnapshot snapshot = metadataCacheProcessingService
            .GetMetadataCacheSnapshot();

        if (!metadataCacheSourceProcessingService
            .IsCurrentMetadataCacheSnapshot(
                metadataCacheSnapshot: snapshot))
        {
            snapshot = RebuildCore();
        }

        return snapshot;
    }

    private MetadataCacheSnapshot RebuildCore()
    {
        MetadataCacheSnapshot snapshot = metadataCacheSourceProcessingService
            .BuildMetadataCacheSnapshot();

        metadataCacheProcessingService.SetMetadataCacheSnapshot(
            metadataCacheSnapshot: snapshot);

        return snapshot;
    }
}