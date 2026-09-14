// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class PackageEventProcessingService(IPackageEventService eventService)
    : IPackageEventProcessingService
{
    public ValueTask RaisePackageAddEventAsync(Package package, string userId) =>
        TryCatch(operation: () =>
    {
        ValidateRaisePackageAddEventAsync(inputs: [package, userId]);

        return eventService.RaisePackageAddEventAsync(
            entity: package,
            userId: userId);

    }, isValueTask: true);

    public ValueTask RaisePackageUpdateEventAsync(Package package, string userId) =>
        TryCatch(operation: () =>
    {
        ValidateRaisePackageUpdateEventAsync(inputs: [package, userId]);

        return eventService.RaisePackageUpdateEventAsync(
            entity: package,
            userId: userId);

    }, isValueTask: true);

    public ValueTask RaisePackageDeleteEventAsync(Package package, string userId) =>
        TryCatch(operation: () =>
    {
        ValidateRaisePackageDeleteEventAsync(inputs: [package, userId]);

        return eventService.RaisePackageDeleteEventAsync(
            entity: package,
            userId: userId);

    }, isValueTask: true);
}