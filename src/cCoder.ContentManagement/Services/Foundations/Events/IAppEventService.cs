// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal interface IAppEventService
{
    ValueTask RaiseAppAddEventAsync(App app);

    ValueTask RaiseAppDeleteEventAsync(App app);

    ValueTask RaiseAppUpdateEventAsync(App app);
}