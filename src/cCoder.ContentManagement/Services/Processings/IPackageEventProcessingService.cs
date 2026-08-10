// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Services.Processings;

internal interface IPackageEventProcessingService
{
    ValueTask RaisePackageImportEvent(int appId, Package package);
    ValueTask RaisePackageImportCompleteEvent(int appId, Package package);
    ValueTask RaisePackagePageRolesImportEvent(int appId, Package package);

    ValueTask RaisePackageAddEventAsync(Package entity);

    ValueTask RaisePackageUpdateEventAsync(Package entity);

    ValueTask RaisePackageDeleteEventAsync(Package entity);
}