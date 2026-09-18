// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Models.PageRendering;
using cCoder.Eventing;
using cCoder.Eventing.Models;

namespace cCoder.ContentManagement.Brokers.Events;

internal sealed class RenderEventBroker(IEventHub eventHub)
    : IRenderEventBroker
{
    public ValueTask RaiseHttpPageRenderOperationRenderRequestAsync(
        EventMessage<HttpPageRenderOperation> message) =>
        eventHub.RaiseEventAsync(
            name: "render_request",
            message: message);

    public ValueTask RaiseTagHandlingOperationRenderTagsAsync(
        EventMessage<TagHandlingOperation> message) =>
        eventHub.RaiseEventAsync(
            name: "render_tags",
            message: message);
}