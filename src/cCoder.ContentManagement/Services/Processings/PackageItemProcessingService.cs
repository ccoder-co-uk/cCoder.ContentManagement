using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.Data.Models.Packaging;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

internal class PackageItemProcessingService(IPackageItemService service) : IPackageItemProcessingService
{
    public PackageItem Get(Guid id) =>
        service.Get(id);

    public IQueryable<PackageItem> GetAll(bool ignoreFilters = false) =>
        service.GetAll(ignoreFilters);

    public ValueTask<PackageItem> AddAsync(PackageItem entity) =>
        service.AddAsync(entity);

    public ValueTask<PackageItem> UpdateAsync(PackageItem entity) =>
        service.UpdateAsync(entity);

    public ValueTask DeleteAsync(Guid id) =>
        service.DeleteAsync(id);

    public async ValueTask<IEnumerable<Result<PackageItem>>> AddOrUpdate(IEnumerable<PackageItem> items)
    {
        List<Result<PackageItem>> results = new List<Result<PackageItem>>();
        foreach (PackageItem item in items)
        {
            try
            {
                PackageItem savedItem = item.Id == Guid.Empty ? await AddAsync(item) : await UpdateAsync(item);
                results.Add(new Result<PackageItem>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id == Guid.Empty ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(new Result<PackageItem>
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
            await DeleteAsync(item.Id);
    }
}
