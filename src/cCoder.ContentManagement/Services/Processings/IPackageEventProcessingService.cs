// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Services.Processings;

internal interface IPackageEventProcessingService
{
    ValueTask RaisePackageAddEventAsync(Package entity, string userId);

    ValueTask RaisePackageUpdateEventAsync(Package entity, string userId);

    ValueTask RaisePackageDeleteEventAsync(Package entity, string userId);
}