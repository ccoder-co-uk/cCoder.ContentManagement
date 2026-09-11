// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class PackageItemEventProcessingService(IPackageItemEventService eventService) : IPackageItemEventProcessingService
{
    public ValueTask RaisePackageItemAddEventAsync(PackageItem packageItem) =>
        TryCatch(operation: () =>
    {
        ValidateRaisePackageItemAddEventAsync(inputs: [packageItem]);
        return eventService.RaisePackageItemAddEventAsync(entity: packageItem);
    }, isValueTask: true);

    public ValueTask RaisePackageItemUpdateEventAsync(PackageItem packageItem) =>
        TryCatch(operation: () =>
    {
        ValidateRaisePackageItemUpdateEventAsync(inputs: [packageItem]);
        return eventService.RaisePackageItemUpdateEventAsync(entity: packageItem);
    }, isValueTask: true);

    public ValueTask RaisePackageItemDeleteEventAsync(PackageItem packageItem) =>
        TryCatch(operation: () =>
    {
        ValidateRaisePackageItemDeleteEventAsync(inputs: [packageItem]);
        return eventService.RaisePackageItemDeleteEventAsync(entity: packageItem);
    }, isValueTask: true);
}