// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.PageRendering;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal interface ITagHandlingOperationEventService
{
    ValueTask RaiseTagHandlingOperationRenderTagsAsync(
        TagHandlingOperation tagHandlingOperation,
        string userId);
}