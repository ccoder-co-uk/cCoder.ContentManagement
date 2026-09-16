// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Caching;
using cCoder.ContentManagement.Models.Caching;

namespace cCoder.ContentManagement.Rendering.Services.Foundations;

internal sealed partial class MetadataCacheService(
    ICacheBroker cacheBroker) : IMetadataCacheService
{
    private const string CacheKey = "ContentManagement.Metadata";

    public MetadataCacheSnapshot GetMetadataCacheSnapshot() =>
        TryCatch(operation: () => cacheBroker.Get<MetadataCacheSnapshot>(
            key: CacheKey));

    public void SetMetadataCacheSnapshot(
        MetadataCacheSnapshot metadataCacheSnapshot) =>
        TryCatch(operation: () =>
        {
            ValidateMetadataCacheSnapshotOnSet(
                inputs: [metadataCacheSnapshot]);

            cacheBroker.Set(
                key: CacheKey,
                value: metadataCacheSnapshot,
                expiry: TimeSpan.FromDays(value: 365));
        });
}