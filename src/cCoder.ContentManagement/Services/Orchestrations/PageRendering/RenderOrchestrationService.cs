// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.PageRendering;
using cCoder.ContentManagement.Rendering.Services.Processings;

namespace cCoder.ContentManagement.Rendering.Services.Orchestrations;

internal sealed partial class RenderOrchestrationService(
    IMetadataCacheProcessingService metadataCacheProcessingService,
    ICommonObjectCacheProcessingService commonObjectCacheProcessingService,
    IMarkupRenderProcessingService markupRenderProcessingService) : IRenderOrchestrationService
{
    public RenderSession RenderRenderSession(RenderSession renderSession) =>
        TryCatch<RenderSession>(operation: () =>
    {
        ValidateRenderRenderSession(inputs: [renderSession]);

        renderSession = metadataCacheProcessingService
            .PrepareRenderSession(session: renderSession);

        renderSession = commonObjectCacheProcessingService
            .PrepareRenderSession(session: renderSession);

        return markupRenderProcessingService.RenderRenderSession(
            session: renderSession);

    });
}