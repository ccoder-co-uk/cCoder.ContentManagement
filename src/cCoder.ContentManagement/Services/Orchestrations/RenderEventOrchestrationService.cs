// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Processings;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal sealed partial class RenderEventOrchestrationService(
    IHttpPageRenderOperationEventProcessingService eventProcessingService,
    IAuthorizationProcessingService authorizationProcessingService)
        : IRenderEventOrchestrationService
{
    public ValueTask RaiseHttpPageRenderOperationRenderRequestAsync(
        HttpPageRenderOperation httpPageRenderOperation) =>
        TryCatch(operation: async () =>
    {
        ValidateRaiseHttpPageRenderOperationRenderRequestAsync(
            inputs: [httpPageRenderOperation]);

        await eventProcessingService
            .RaiseHttpPageRenderOperationRenderRequestAsync(
            httpPageRenderOperation: httpPageRenderOperation,
            userId: authorizationProcessingService.GetCurrentUserId());
    }, isValueTask: true);
}