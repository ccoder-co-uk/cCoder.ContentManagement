// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal interface ILayoutEventService
{
    ValueTask RaiseLayoutAddEventAsync(Layout entity, string userId);

    ValueTask RaiseLayoutUpdateEventAsync(Layout entity, string userId);

    ValueTask RaiseLayoutDeleteEventAsync(Layout entity, string userId);
}