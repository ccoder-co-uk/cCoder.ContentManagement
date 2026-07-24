// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.Data.Models.Packaging;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

internal class PackageItemProcessingService(IPackageItemService service) : IPackageItemProcessingService
{
    public PackageItem GetPackageItem(Guid packageItemId) =>
        service.GetPackageItem(packageItemId: packageItemId);

    public IQueryable<PackageItem> GetAllPackageItem(bool ignoreFilters = false) =>
        service.GetAllPackageItem(ignoreFilters: ignoreFilters);

    public ValueTask<PackageItem> AddPackageItemAsync(PackageItem newPackageItem) =>
        service.AddPackageItemAsync(newPackageItem: newPackageItem);

    public ValueTask<PackageItem> UpdatePackageItemAsync(PackageItem updatedPackageItem) =>
        service.UpdatePackageItemAsync(updatedPackageItem: updatedPackageItem);

    public ValueTask DeleteAsync(Guid packageItemId) =>
        service.DeleteAsync(packageItemId: packageItemId);

    public async ValueTask<IEnumerable<Result<PackageItem>>> AddOrUpdatePackageItemResult(IEnumerable<PackageItem> newPackageItem)
    {
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
    }

    public async ValueTask DeleteAllPackageItemAsync(IEnumerable<PackageItem> deletedPackageItem)
    {
        foreach (PackageItem item in deletedPackageItem)
        {
            await ExecuteDeleteAsync(packageItemId: item.Id);
        }
    }

    private ValueTask<PackageItem> ExecuteAddPackageItemAsync(PackageItem newPackageItem) =>
        service.AddPackageItemAsync(newPackageItem: newPackageItem);
    private ValueTask ExecuteDeleteAsync(Guid packageItemId) =>
        service.DeleteAsync(packageItemId: packageItemId);
    private ValueTask<PackageItem> ExecuteUpdatePackageItemAsync(PackageItem updatedPackageItem) =>
        service.UpdatePackageItemAsync(updatedPackageItem: updatedPackageItem);
}