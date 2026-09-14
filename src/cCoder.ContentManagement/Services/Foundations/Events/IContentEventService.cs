// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal interface IContentEventService
{
    ValueTask RaiseContentAddEventAsync(Content entity, string userId);

    ValueTask RaiseContentUpdateEventAsync(Content entity, string userId);

    ValueTask RaiseContentDeleteEventAsync(Content entity, string userId);
}