// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.PageRendering;
using cCoder.ContentManagement.Rendering.Services.Foundations;

namespace cCoder.ContentManagement.Rendering.Services.Processings;

internal sealed partial class MetadataRenderCacheProcessingService(
    IMetadataRenderCacheService metadataRenderCacheService)
        : IMetadataRenderCacheProcessingService
{
    public RenderSession PrepareRenderSession(RenderSession renderSession) =>
        TryCatch(operation: () =>
        {
            ValidatePrepareRenderSession(inputs: [renderSession]);

            string culture = !string.IsNullOrWhiteSpace(
                value: renderSession.Request.Culture)
                    ? renderSession.Request.Culture
                    : renderSession.App?.DefaultCulture ?? string.Empty;

            renderSession.MetadataResolver = metadataRenderCacheService.Get(
                culture: culture);

            return renderSession;
        });
}