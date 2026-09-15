// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal interface IComponentEventService
{
    ValueTask RaiseComponentAddEventAsync(Component entity, string userId);

    ValueTask RaiseComponentUpdateEventAsync(Component entity, string userId);

    ValueTask RaiseComponentDeleteEventAsync(Component entity, string userId);
}