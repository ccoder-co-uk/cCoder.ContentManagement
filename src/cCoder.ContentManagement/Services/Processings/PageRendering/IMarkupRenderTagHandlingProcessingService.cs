// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.PageRendering;

namespace cCoder.ContentManagement.Rendering.Services.Processings;

internal interface IMarkupRenderTagHandlingProcessingService
{
    ValueTask RenderCultureLinkTagHandlingOperationAsync(TagHandlingOperation tagHandlingOperation);
    ValueTask RenderMetadataTagHandlingOperationAsync(TagHandlingOperation tagHandlingOperation);
    ValueTask RenderNavigationTagHandlingOperationAsync(TagHandlingOperation tagHandlingOperation);
    ValueTask RenderContentTagHandlingOperationAsync(TagHandlingOperation tagHandlingOperation);
    ValueTask RenderComponentTagHandlingOperationAsync(TagHandlingOperation tagHandlingOperation);
    ValueTask RenderScriptTagHandlingOperationAsync(TagHandlingOperation tagHandlingOperation);
    ValueTask RenderStyleTagHandlingOperationAsync(TagHandlingOperation tagHandlingOperation);
    ValueTask RenderReplacementTagHandlingOperationAsync(TagHandlingOperation tagHandlingOperation);
    ValueTask RenderDmsTagHandlingOperationAsync(TagHandlingOperation tagHandlingOperation);
    ValueTask RenderResourceTagHandlingOperationAsync(TagHandlingOperation tagHandlingOperation);
    ValueTask RenderExecuteTagHandlingOperationAsync(TagHandlingOperation tagHandlingOperation);
}