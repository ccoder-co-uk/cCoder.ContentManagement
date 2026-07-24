// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.Data.Models.Packaging;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class PackageItemEventProcessingService(IPackageItemEventService eventService) : IPackageItemEventProcessingService
{
    public ValueTask RaisePackageItemAddEventAsync(PackageItem entity) =>
        TryCatch(operation: () =>
    {
        ValidateRaisePackageItemAddEventAsync(inputs: [entity]);
        return eventService.RaisePackageItemAddEventAsync(entity: entity);
    }, isValueTask: true);

    public ValueTask RaisePackageItemUpdateEventAsync(PackageItem entity) =>
        TryCatch(operation: () =>
    {
        ValidateRaisePackageItemUpdateEventAsync(inputs: [entity]);
        return eventService.RaisePackageItemUpdateEventAsync(entity: entity);
    }, isValueTask: true);

    public ValueTask RaisePackageItemDeleteEventAsync(PackageItem entity) =>
        TryCatch(operation: () =>
    {
        ValidateRaisePackageItemDeleteEventAsync(inputs: [entity]);
        return eventService.RaisePackageItemDeleteEventAsync(entity: entity);
    }, isValueTask: true);
}