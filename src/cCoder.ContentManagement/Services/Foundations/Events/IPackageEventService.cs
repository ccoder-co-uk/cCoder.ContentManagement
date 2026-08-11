// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal interface IPackageEventService
{
    ValueTask RaisePackageAddEventAsync(Package entity);

    ValueTask RaisePackageUpdateEventAsync(Package entity);

    ValueTask RaisePackageDeleteEventAsync(Package entity);
}