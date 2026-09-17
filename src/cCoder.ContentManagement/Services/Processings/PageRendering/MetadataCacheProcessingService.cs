// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Caching;
using cCoder.ContentManagement.Rendering.Services.Foundations;

namespace cCoder.ContentManagement.Rendering.Services.Processings;

internal sealed partial class MetadataCacheProcessingService(
    IMetadataCacheService metadataCacheService)
        : IMetadataCacheProcessingService
{
    public MetadataCacheSnapshot GetMetadataCacheSnapshot() =>
        TryCatch(operation: () => metadataCacheService
            .GetMetadataCacheSnapshot());

    public void SetMetadataCacheSnapshot(
        MetadataCacheSnapshot metadataCacheSnapshot) =>
        TryCatch(operation: () =>
        {
            ValidateMetadataCacheSnapshotOnSet(
                inputs: [metadataCacheSnapshot]);

            metadataCacheService.SetMetadataCacheSnapshot(
                metadataCacheSnapshot: metadataCacheSnapshot);
        });
}