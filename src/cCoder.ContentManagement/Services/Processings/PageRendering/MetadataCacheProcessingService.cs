// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.PageRendering;
using cCoder.ContentManagement.Rendering.Services.Foundations;

namespace cCoder.ContentManagement.Rendering.Services.Processings;

internal sealed partial class MetadataCacheProcessingService(
    IMetadataCacheService metadataCacheService) : IMetadataCacheProcessingService
{
    public RenderSession PrepareRenderSession(RenderSession session) =>
        TryCatch(operation: () =>
    {
        ValidatePrepareRenderSession(inputs: [session]);

        string culture = !string.IsNullOrWhiteSpace(value: session.Request.Culture)
            ? session.Request.Culture
            : session.App?.DefaultCulture ?? string.Empty;

        session.MetadataResolver = metadataCacheService.Get(culture: culture);
        return session;
    });
}