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
    public RenderSession RenderRenderSession(RenderSession session) =>
        TryCatch<RenderSession>(operation: () =>
    {
        ValidateRenderRenderSession(inputs: [session]);

        session = metadataCacheProcessingService
            .PrepareRenderSession(session: session);

        session = commonObjectCacheProcessingService
            .PrepareRenderSession(session: session);

        return markupRenderProcessingService.RenderRenderSession(
            session: session);

    });
}