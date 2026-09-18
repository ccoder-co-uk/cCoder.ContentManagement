// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Models.PageRendering;
using cCoder.ContentManagement.Rendering.Services.Processings;
using cCoder.ContentManagement.Services.Processings;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal sealed partial class MarkupRenderOrchestrationService(
    IMarkupRenderProcessingService markupRenderProcessingService,
    ITagHandlingOperationEventProcessingService eventProcessingService)
        : IMarkupRenderOrchestrationService
{
    public ValueTask RenderHttpPageRenderOperationAsync(
        HttpPageRenderOperation httpPageRenderOperation) =>
        TryCatch(operation: async () =>
    {
        ValidateRenderHttpPageRenderOperationAsync(
            inputs: [httpPageRenderOperation]);

        if (httpPageRenderOperation.Failure != HttpPageRenderFailure.None)
        {
            return;
        }

        PageRenderOperation pageRenderOperation =
            httpPageRenderOperation.RenderOperation;

        pageRenderOperation.RenderSession.TagHandler =
            HandleTagHandlingOperationAsync;

        pageRenderOperation.RenderSession = await markupRenderProcessingService
            .RenderRenderSessionAsync(
                renderSession: pageRenderOperation.RenderSession);

        ValueTask HandleTagHandlingOperationAsync(
            TagHandlingOperation tagHandlingOperation) =>
            eventProcessingService.RaiseTagHandlingOperationRenderTagsAsync(
                tagHandlingOperation: tagHandlingOperation,
                userId: pageRenderOperation.User?.Id ?? "Guest");

    }, isValueTask: true);
}