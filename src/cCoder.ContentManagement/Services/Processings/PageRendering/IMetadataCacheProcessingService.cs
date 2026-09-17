// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.PageRendering;
using cCoder.ContentManagement.Models.Caching;

namespace cCoder.ContentManagement.Rendering.Services.Processings;

internal interface IMetadataRenderCacheProcessingService
{
    RenderSession PrepareRenderSession(RenderSession session);
}

internal interface IMetadataCacheProcessingService
{
    MetadataCacheSnapshot GetMetadataCacheSnapshot();

    void SetMetadataCacheSnapshot(MetadataCacheSnapshot metadataCacheSnapshot);
}

internal interface IMetadataCacheSourceProcessingService
{
    MetadataCacheSnapshot BuildMetadataCacheSnapshot();

    bool IsCurrentMetadataCacheSnapshot(
        MetadataCacheSnapshot metadataCacheSnapshot);
}