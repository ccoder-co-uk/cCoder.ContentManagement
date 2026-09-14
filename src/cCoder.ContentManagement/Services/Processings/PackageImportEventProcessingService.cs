// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Foundations.Events;

namespace cCoder.ContentManagement.Services.Processings;

internal sealed partial class PackageImportEventProcessingService(
    IPackageImportEventService packageImportEventService)
    : IPackageImportEventProcessingService
{
    public ValueTask RaiseImportAsync<T>(
        string eventName,
        PackageItemImportEvent<T> import,
        string userId) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseImportAsync(inputs: [eventName, import, userId]);

        return packageImportEventService.RaiseImportAsync(
            eventName: eventName,
            import: import,
            userId: userId);

    }, isValueTask: true);
}