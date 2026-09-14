// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal interface IPageEventService
{
    ValueTask RaisePageAddEventAsync(Page entity, string userId);

    ValueTask RaisePageUpdateEventAsync(Page entity, string userId);

    ValueTask RaisePageDeleteEventAsync(Page entity, string userId);

}