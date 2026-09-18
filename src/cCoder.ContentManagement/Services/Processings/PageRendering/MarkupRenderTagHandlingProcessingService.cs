// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.PageRendering;
using cCoder.ContentManagement.Rendering.Services.Foundations;

namespace cCoder.ContentManagement.Rendering.Services.Processings;

internal sealed partial class MarkupRenderTagHandlingProcessingService(
    IMarkupRenderService markupRenderService)
        : IMarkupRenderTagHandlingProcessingService
{
    public ValueTask RenderCultureLinkTagHandlingOperationAsync(TagHandlingOperation tagHandlingOperation) =>
        TryCatch(operation: () =>
    {
        ValidateTagHandlingOperationOnRender(inputs: [tagHandlingOperation]);

        return HandleTagHandlingOperation(tagHandlingOperation: tagHandlingOperation, handler: markupRenderService.RenderCultureLinkTagHandlingOperation);
    }, isValueTask: true);

    public ValueTask RenderMetadataTagHandlingOperationAsync(TagHandlingOperation tagHandlingOperation) =>
        TryCatch(operation: () =>
    {
        ValidateTagHandlingOperationOnRender(inputs: [tagHandlingOperation]);

        return HandleTagHandlingOperation(tagHandlingOperation: tagHandlingOperation, handler: markupRenderService.RenderMetadataTagHandlingOperation);
    }, isValueTask: true);

    public ValueTask RenderNavigationTagHandlingOperationAsync(TagHandlingOperation tagHandlingOperation) =>
        TryCatch(operation: () =>
    {
        ValidateTagHandlingOperationOnRender(inputs: [tagHandlingOperation]);

        return HandleTagHandlingOperation(tagHandlingOperation: tagHandlingOperation, handler: markupRenderService.RenderNavigationTagHandlingOperation);
    }, isValueTask: true);

    public ValueTask RenderContentTagHandlingOperationAsync(TagHandlingOperation tagHandlingOperation) =>
        TryCatch(operation: () =>
    {
        ValidateTagHandlingOperationOnRender(inputs: [tagHandlingOperation]);

        return HandleTagHandlingOperation(tagHandlingOperation: tagHandlingOperation, handler: markupRenderService.RenderContentTagHandlingOperation);
    }, isValueTask: true);

    public ValueTask RenderComponentTagHandlingOperationAsync(TagHandlingOperation tagHandlingOperation) =>
        TryCatch(operation: () =>
    {
        ValidateTagHandlingOperationOnRender(inputs: [tagHandlingOperation]);

        return HandleTagHandlingOperation(tagHandlingOperation: tagHandlingOperation, handler: markupRenderService.RenderComponentTagHandlingOperation);
    }, isValueTask: true);

    public ValueTask RenderScriptTagHandlingOperationAsync(TagHandlingOperation tagHandlingOperation) =>
        TryCatch(operation: () =>
    {
        ValidateTagHandlingOperationOnRender(inputs: [tagHandlingOperation]);

        return HandleTagHandlingOperation(tagHandlingOperation: tagHandlingOperation, handler: markupRenderService.RenderScriptTagHandlingOperation);
    }, isValueTask: true);

    public ValueTask RenderStyleTagHandlingOperationAsync(TagHandlingOperation tagHandlingOperation) =>
        TryCatch(operation: () =>
    {
        ValidateTagHandlingOperationOnRender(inputs: [tagHandlingOperation]);

        return HandleTagHandlingOperation(tagHandlingOperation: tagHandlingOperation, handler: markupRenderService.RenderStyleTagHandlingOperation);
    }, isValueTask: true);

    public ValueTask RenderReplacementTagHandlingOperationAsync(TagHandlingOperation tagHandlingOperation) =>
        TryCatch(operation: () =>
    {
        ValidateTagHandlingOperationOnRender(inputs: [tagHandlingOperation]);

        return HandleTagHandlingOperation(tagHandlingOperation: tagHandlingOperation, handler: MarkupRenderProcessingService.RenderReplacementTagHandlingOperation);
    }, isValueTask: true);

    public ValueTask RenderDmsTagHandlingOperationAsync(TagHandlingOperation tagHandlingOperation) =>
        TryCatch(operation: () =>
    {
        ValidateTagHandlingOperationOnRender(inputs: [tagHandlingOperation]);

        return HandleTagHandlingOperation(tagHandlingOperation: tagHandlingOperation, handler: markupRenderService.RenderDmsTagHandlingOperation);
    }, isValueTask: true);

    public ValueTask RenderResourceTagHandlingOperationAsync(TagHandlingOperation tagHandlingOperation) =>
        TryCatch(operation: () =>
    {
        ValidateTagHandlingOperationOnRender(inputs: [tagHandlingOperation]);

        return HandleTagHandlingOperation(tagHandlingOperation: tagHandlingOperation, handler: markupRenderService.RenderResourceTagHandlingOperation);
    }, isValueTask: true);

    public ValueTask RenderExecuteTagHandlingOperationAsync(TagHandlingOperation tagHandlingOperation) =>
        TryCatch(operation: () =>
    {
        ValidateTagHandlingOperationOnRender(inputs: [tagHandlingOperation]);

        return HandleTagHandlingOperation(tagHandlingOperation: tagHandlingOperation, handler: markupRenderService.RenderExecuteTagHandlingOperation);
    }, isValueTask: true);

    private static ValueTask HandleTagHandlingOperation(
        TagHandlingOperation tagHandlingOperation,
        Func<TagHandlingOperation, TagHandlingOperation> handler)
    {
        ValidateTagHandlingOperationOnRender(inputs: [tagHandlingOperation]);
        _ = handler(arg: tagHandlingOperation);

        return ValueTask.CompletedTask;
    }
}