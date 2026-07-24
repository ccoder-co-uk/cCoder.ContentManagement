// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Services.Processings;

internal class PackageEventProcessingService(IPackageEventService eventService) : IPackageEventProcessingService
{
    public ValueTask RaisePackageImportEvent(int appId, Package package) =>
        eventService.RaisePackageImportEventAsync(appId: appId, package: package);

    public ValueTask RaisePackageAddEventAsync(Package package) =>
        eventService.RaisePackageAddEventAsync(entity: package);

    public ValueTask RaisePackageUpdateEventAsync(Package package) =>
        eventService.RaisePackageUpdateEventAsync(entity: package);

    public ValueTask RaisePackageDeleteEventAsync(Package package) =>
        eventService.RaisePackageDeleteEventAsync(entity: package);
}