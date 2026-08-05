// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal interface IUncachedPageRenderEventService
{
    ValueTask RaiseUncachedPageRenderEventAsync(
        UncachedPageRenderEvent pageRenderEvent);
}