// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.Data.Models.Packaging;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class PackageItemProcessingService(IPackageItemService service) : IPackageItemProcessingService
{
    public PackageItem GetPackageItem(Guid packageItemId) =>
        TryCatch<PackageItem>(operation: () =>
    {
        ValidatePackageItemOnGet(inputs: [packageItemId]);
        return service.GetPackageItem(packageItemId: packageItemId);
    });

    public IQueryable<PackageItem> GetAllPackageItem(bool ignoreFilters = false) =>
        TryCatch<IQueryable<PackageItem>>(operation: () =>
    {
        ValidateAllPackageItemOnGet(inputs: [ignoreFilters]);
        return service.GetAllPackageItem(ignoreFilters: ignoreFilters);
    });

    public ValueTask<PackageItem> AddPackageItemAsync(PackageItem newPackageItem) =>
        TryCatch<PackageItem>(operation: () =>
    {
        ValidatePackageItemOnAdd(inputs: [newPackageItem]);
        return service.AddPackageItemAsync(newPackageItem: newPackageItem);
    }, isValueTask: true);

    public ValueTask<PackageItem> UpdatePackageItemAsync(PackageItem updatedPackageItem) =>
        TryCatch<PackageItem>(operation: () =>
    {
        ValidatePackageItemOnUpdate(inputs: [updatedPackageItem]);
        return service.UpdatePackageItemAsync(updatedPackageItem: updatedPackageItem);
    }, isValueTask: true);

    public ValueTask DeleteAsync(Guid packageItemId) =>
        TryCatch(operation: () =>
    {
        ValidateDeleteAsync(inputs: [packageItemId]);
        return service.DeleteAsync(packageItemId: packageItemId);
    }, isValueTask: true);

    public ValueTask<IEnumerable<Result<PackageItem>>> AddOrUpdatePackageItemResult(IEnumerable<PackageItem> newPackageItem) =>
        TryCatch<IEnumerable<Result<PackageItem>>>(operation: async () =>
    {
        ValidateOrUpdatePackageItemResultOnAdd(inputs: [newPackageItem]);
        List<Result<PackageItem>> results = new List<Result<PackageItem>>();

        foreach (PackageItem item in newPackageItem)
        {
            try
            {
                PackageItem savedItem = item.Id == Guid.Empty ? await ExecuteAddPackageItemAsync(newPackageItem: item) : await ExecuteUpdatePackageItemAsync(updatedPackageItem: item);

                results.Add(item: new Result<PackageItem>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id == Guid.Empty ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new Result<PackageItem>
                {
                    Success = false,
                    Item = item,
                    Message = ex.Message
                });
            }
        }

        return results;

    }, isValueTask: true);

    public ValueTask DeleteAllPackageItemAsync(IEnumerable<PackageItem> deletedPackageItem) =>
        TryCatch(operation: async () =>
    {
        ValidateAllPackageItemOnDelete(inputs: [deletedPackageItem]);

        foreach (PackageItem item in deletedPackageItem)
        {
            await ExecuteDeleteAsync(packageItemId: item.Id);
        }

    }, isValueTask: true);

    private ValueTask<PackageItem> ExecuteAddPackageItemAsync(PackageItem newPackageItem) =>
        service.AddPackageItemAsync(newPackageItem: newPackageItem);

    private ValueTask ExecuteDeleteAsync(Guid packageItemId) =>
        service.DeleteAsync(packageItemId: packageItemId);

    private ValueTask<PackageItem> ExecuteUpdatePackageItemAsync(PackageItem updatedPackageItem) =>
        service.UpdatePackageItemAsync(updatedPackageItem: updatedPackageItem);
}