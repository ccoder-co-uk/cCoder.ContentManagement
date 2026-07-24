// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Events;

public interface ILayoutEventService
{
    ValueTask RaiseLayoutAddEventAsync(Layout entity);

    ValueTask RaiseLayoutUpdateEventAsync(Layout entity);

    ValueTask RaiseLayoutDeleteEventAsync(Layout entity);
}