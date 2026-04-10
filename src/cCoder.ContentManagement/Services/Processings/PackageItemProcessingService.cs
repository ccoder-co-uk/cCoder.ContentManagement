using cCoder.ContentManagement.Services.Foundations.Storages;
using PackageItem = cCoder.Data.Models.Packaging.PackageItem;

namespace cCoder.ContentManagement.Services.Processings;

internal class PackageItemProcessingService(IPackageItemService service) : IPackageItemProcessingService
{
    public PackageItem Get(Guid id)
    {
        return service.Get(id);
    }

    public IQueryable<PackageItem> GetAll(bool ignoreFilters = false)
    {
        return service.GetAll(ignoreFilters);
    }

    public ValueTask<PackageItem> AddAsync(PackageItem entity)
    {
        return service.AddAsync(entity);
    }

    public ValueTask<PackageItem> UpdateAsync(PackageItem entity)
    {
        return service.UpdateAsync(entity);
    }

    public ValueTask DeleteAsync(Guid id)
    {
        return service.DeleteAsync(id);
    }

    public async ValueTask<IEnumerable<cCoder.ContentManagement.Models.Result<PackageItem>>> AddOrUpdate(IEnumerable<PackageItem> items)
    {
        List<cCoder.ContentManagement.Models.Result<PackageItem>> results = new List<cCoder.ContentManagement.Models.Result<PackageItem>>();
        foreach (PackageItem item in items)
        {
            try
            {
                PackageItem savedItem = item.Id == Guid.Empty ? await AddAsync(item) : await UpdateAsync(item);
                results.Add(new cCoder.ContentManagement.Models.Result<PackageItem>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id == Guid.Empty ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(new cCoder.ContentManagement.Models.Result<PackageItem>
                {
                    Success = false,
                    Item = item,
                    Message = ex.Message
                });
            }
        }
        return results;
    }

    public async ValueTask DeleteAllAsync(IEnumerable<PackageItem> items)
    {
        foreach (PackageItem item in items)
        {
            await DeleteAsync(item.Id);
        }
    }
}
