// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Processings.PageRendering;

namespace cCoder.ContentManagement.Rendering.Brokers;

internal interface IRenderBroker
{
    IEnumerable<ITagHandlingProcessingService> GetTagHandlers();
}