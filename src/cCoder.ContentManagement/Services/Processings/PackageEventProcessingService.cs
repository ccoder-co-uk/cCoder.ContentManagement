// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class PackageEventProcessingService(IPackageEventService eventService) : IPackageEventProcessingService
{
    public ValueTask RaisePackageImportEvent(int appId, Package package) =>
        TryCatch(operation: () =>
    {
        ValidateRaisePackageImportEvent(inputs: [appId, package]);
        return eventService.RaisePackageImportEventAsync(appId: appId, package: package);
    }, isValueTask: true);

    public ValueTask RaisePackageImportCompleteEvent(int appId, Package package) =>
        TryCatch(operation: () =>
    {
        ValidateRaisePackageImportEvent(inputs: [appId, package]);
        return eventService.RaisePackageImportCompleteEventAsync(appId: appId, package: package);
    }, isValueTask: true);

    public ValueTask RaisePackageAddEventAsync(Package package) =>
        TryCatch(operation: () =>
    {
        ValidateRaisePackageAddEventAsync(inputs: [package]);
        return eventService.RaisePackageAddEventAsync(entity: package);
    }, isValueTask: true);

    public ValueTask RaisePackageUpdateEventAsync(Package package) =>
        TryCatch(operation: () =>
    {
        ValidateRaisePackageUpdateEventAsync(inputs: [package]);
        return eventService.RaisePackageUpdateEventAsync(entity: package);
    }, isValueTask: true);

    public ValueTask RaisePackageDeleteEventAsync(Package package) =>
        TryCatch(operation: () =>
    {
        ValidateRaisePackageDeleteEventAsync(inputs: [package]);
        return eventService.RaisePackageDeleteEventAsync(entity: package);
    }, isValueTask: true);
}