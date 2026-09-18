// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Models.PageRendering;
using cCoder.Eventing.Models;

namespace cCoder.ContentManagement.Brokers.Events;

internal interface IRenderEventBroker
{
    ValueTask RaiseHttpPageRenderOperationRenderRequestAsync(
        EventMessage<HttpPageRenderOperation> message);

    ValueTask RaiseTagHandlingOperationRenderTagsAsync(
        EventMessage<TagHandlingOperation> message);
}