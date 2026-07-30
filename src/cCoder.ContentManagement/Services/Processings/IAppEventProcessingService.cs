// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal interface IAppEventProcessingService
{
    ValueTask RaiseAppAddEventAsync(App app);

    ValueTask RaiseAppDeleteEventAsync(App app);

    ValueTask RaiseAppUpdateEventAsync(App app);
}