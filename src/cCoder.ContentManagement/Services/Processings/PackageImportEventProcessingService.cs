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
        PackageItemImportEvent<T> import) =>
        TryCatch(operation: () =>
    {
        ValidateRaiseImportAsync(inputs: [eventName, import]);

        return packageImportEventService.RaiseImportAsync(
            eventName: eventName,
            import: import);
    }, isValueTask: true);
}