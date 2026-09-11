// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.PageRendering;
using cCoder.ContentManagement.Rendering.Services.Foundations;

namespace cCoder.ContentManagement.Rendering.Services.Processings;

internal sealed partial class MetadataCacheProcessingService(
    IMetadataCacheService metadataCacheService) : IMetadataCacheProcessingService
{
    public RenderSession PrepareRenderSession(RenderSession renderSession) =>
        TryCatch(operation: () =>
    {
        ValidatePrepareRenderSession(inputs: [renderSession]);

        string culture = !string.IsNullOrWhiteSpace(value: renderSession.Request.Culture)
            ? renderSession.Request.Culture
            : renderSession.App?.DefaultCulture ?? string.Empty;

        renderSession.MetadataResolver = metadataCacheService.Get(culture: culture);
        return renderSession;
    });
}