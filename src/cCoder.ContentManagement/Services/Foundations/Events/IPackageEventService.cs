// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal interface IPackageEventService
{
    ValueTask RaisePackageAddEventAsync(Package entity, string userId);

    ValueTask RaisePackageUpdateEventAsync(Package entity, string userId);

    ValueTask RaisePackageDeleteEventAsync(Package entity, string userId);
}