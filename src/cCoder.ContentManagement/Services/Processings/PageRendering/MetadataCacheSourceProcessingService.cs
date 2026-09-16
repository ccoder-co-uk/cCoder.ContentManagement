// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.Caching;
using cCoder.ContentManagement.Rendering.Services.Foundations;

namespace cCoder.ContentManagement.Rendering.Services.Processings;

internal sealed partial class MetadataCacheSourceProcessingService(
    IMetadataCacheSourceService metadataCacheSourceService)
        : IMetadataCacheSourceProcessingService
{
    public MetadataCacheSnapshot BuildMetadataCacheSnapshot() =>
        TryCatch(operation: () => metadataCacheSourceService
            .BuildMetadataCacheSnapshot());

    public bool IsCurrentMetadataCacheSnapshot(
        MetadataCacheSnapshot metadataCacheSnapshot) =>
        TryCatch(operation: () =>
        {
            ValidateMetadataCacheSnapshotOnCheck(
                inputs: [metadataCacheSnapshot]);

            return metadataCacheSnapshot is not null
                && string.Equals(
                    a: metadataCacheSnapshot.Signature,
                    b: metadataCacheSourceService.GetMetadataSignature(),
                    comparisonType: StringComparison.Ordinal);
        });
}