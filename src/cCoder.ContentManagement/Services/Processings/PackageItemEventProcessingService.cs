// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class PackageItemEventProcessingService(IPackageItemEventService eventService)
    : IPackageItemEventProcessingService
{
    public ValueTask RaisePackageItemAddEventAsync(PackageItem packageItem, string userId) =>
        TryCatch(operation: () =>
    {
        ValidateRaisePackageItemAddEventAsync(inputs: [packageItem, userId]);

        return eventService.RaisePackageItemAddEventAsync(
            entity: packageItem,
            userId: userId);

    }, isValueTask: true);

    public ValueTask RaisePackageItemUpdateEventAsync(PackageItem packageItem, string userId) =>
        TryCatch(operation: () =>
    {
        ValidateRaisePackageItemUpdateEventAsync(inputs: [packageItem, userId]);

        return eventService.RaisePackageItemUpdateEventAsync(
            entity: packageItem,
            userId: userId);

    }, isValueTask: true);

    public ValueTask RaisePackageItemDeleteEventAsync(PackageItem packageItem, string userId) =>
        TryCatch(operation: () =>
    {
        ValidateRaisePackageItemDeleteEventAsync(inputs: [packageItem, userId]);

        return eventService.RaisePackageItemDeleteEventAsync(
            entity: packageItem,
            userId: userId);

    }, isValueTask: true);
}