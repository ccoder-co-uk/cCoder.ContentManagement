// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models.Packaging;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal partial class PackageItemOrchestrationService(
    IPackageItemProcessingService processingService,
    IPackageItemEventProcessingService eventService,
    IAuthorizationProcessingService authorizationProcessingService)
        : IPackageItemOrchestrationService
{
    public PackageItem GetPackageItem(Guid packageItemId) =>
        TryCatch<PackageItem>(operation: () =>
    {
        ValidatePackageItemOnGet(inputs: [packageItemId]);
        return processingService.GetPackageItem(packageItemId: packageItemId);
    });

    public IQueryable<PackageItem> GetAllPackageItem(bool ignoreFilters = false) =>
        TryCatch<IQueryable<PackageItem>>(operation: () =>
    {
        ValidateAllPackageItemOnGet(inputs: [ignoreFilters]);
        return processingService.GetAllPackageItem(ignoreFilters: ignoreFilters);
    });

    public ValueTask<PackageItem> AddPackageItemAsync(PackageItem newPackageItem) =>
        TryCatch<PackageItem>(operation: async () =>
    {
        ValidatePackageItemOnAdd(inputs: [newPackageItem]);
        Authorize(privilege: "PackageItem_create");
        PackageItem result = await processingService.AddPackageItemAsync(newPackageItem: newPackageItem);

        await eventService.RaisePackageItemAddEventAsync(
            entity: result,
            userId: authorizationProcessingService.GetCurrentUserId());

        return result;

    }, isValueTask: true);

    public ValueTask<PackageItem> UpdatePackageItemAsync(PackageItem updatedPackageItem) =>
        TryCatch<PackageItem>(operation: async () =>
    {
        ValidatePackageItemOnUpdate(inputs: [updatedPackageItem]);
        Authorize(privilege: "PackageItem_update");
        PackageItem result = await processingService.UpdatePackageItemAsync(updatedPackageItem: updatedPackageItem);

        await eventService.RaisePackageItemUpdateEventAsync(
            entity: result,
            userId: authorizationProcessingService.GetCurrentUserId());

        return result;

    }, isValueTask: true);

    public ValueTask DeleteAsync(Guid packageItemId) =>
        TryCatch(operation: async () =>
    {
        ValidateDeleteAsync(inputs: [packageItemId]);
        PackageItem entity = processingService.GetPackageItem(packageItemId: packageItemId);
        Authorize(privilege: "PackageItem_delete");

        await eventService.RaisePackageItemDeleteEventAsync(
            entity: entity,
            userId: authorizationProcessingService.GetCurrentUserId());

        await processingService.DeleteAsync(packageItemId: packageItemId);

    }, isValueTask: true);

    public ValueTask<IEnumerable<OperationResult<PackageItem>>> AddOrUpdatePackageItemResult(IEnumerable<PackageItem> newPackageItem) =>
        TryCatch<IEnumerable<OperationResult<PackageItem>>>(operation: () =>
    {
        ValidateOrUpdatePackageItemResultOnAdd(inputs: [newPackageItem]);
        PackageItem[] packageItems = newPackageItem.ToArray();

        foreach (PackageItem packageItem in packageItems)
        {
            Authorize(privilege: packageItem.Id == Guid.Empty
                ? "PackageItem_create"
                : "PackageItem_update");
        }

        return processingService.AddOrUpdatePackageItemResult(newPackageItem: packageItems);
    }, isValueTask: true);

    public ValueTask DeleteAllPackageItemAsync(IEnumerable<PackageItem> deletedPackageItem) =>
        TryCatch(operation: () =>
    {
        ValidateAllPackageItemOnDelete(inputs: [deletedPackageItem]);
        PackageItem[] packageItems = deletedPackageItem.ToArray();

        foreach (PackageItem packageItem in packageItems)
        {
            Authorize(privilege: "PackageItem_delete");
        }

        return processingService.DeleteAllPackageItemAsync(deletedPackageItem: packageItems);
    }, isValueTask: true);

    private void Authorize(string privilege) =>
        authorizationProcessingService.AuthorizeAuthorizationContext(
            context: new AuthorizationContext
            {
                Request = new AuthorizationRequest
                {
                    AppId = null,
                    Privilege = privilege
                }
            });
}
