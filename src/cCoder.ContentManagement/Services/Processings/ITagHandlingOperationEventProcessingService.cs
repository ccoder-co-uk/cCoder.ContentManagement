// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.PageRendering;

namespace cCoder.ContentManagement.Services.Processings;

internal interface ITagHandlingOperationEventProcessingService
{
    ValueTask RaiseTagHandlingOperationRenderTagsAsync(
        TagHandlingOperation tagHandlingOperation,
        string userId);
}