// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Processings.PageRendering;

namespace cCoder.ContentManagement.Rendering.Brokers;

internal sealed class RenderBroker(
    IEnumerable<ITagHandlingProcessingService> tagHandlers)
        : IRenderBroker
{
    public IEnumerable<ITagHandlingProcessingService> GetTagHandlers() =>
        tagHandlers;
}