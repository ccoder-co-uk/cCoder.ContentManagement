using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

internal class PageInfoProcessingService(IPageInfoService service) : IPageInfoProcessingService
{
    public PageInfo Get(int id)
    {
        ValidateId(id, "id");
        return service.Get(id);
    }

    public IQueryable<PageInfo> GetAll(bool ignoreFilters = false) =>
        service.GetAll(ignoreFilters);

    public ValueTask<PageInfo> AddAsync(PageInfo entity)
    {
        ValidatePageInfo(entity, "entity");
        return service.AddAsync(entity);
    }

    public ValueTask<PageInfo> UpdateAsync(PageInfo entity)
    {
        ValidatePageInfo(entity, "entity");
        return service.UpdateAsync(entity);
    }

    public ValueTask DeleteAsync(int id)
    {
        ValidateId(id, "id");
        return service.DeleteAsync(id);
    }

    public async ValueTask<IEnumerable<Result<PageInfo>>> AddOrUpdate(IEnumerable<PageInfo> items)
    {
        ValidatePageInfos(items, "items");
        List<Result<PageInfo>> results = new List<Result<PageInfo>>();
        foreach (PageInfo item in items)
        {
            try
            {
                PageInfo savedItem = item.Id < 1 ? await AddAsync(item) : await UpdateAsync(item);
                results.Add(new Result<PageInfo>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id < 1 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(new Result<PageInfo>
                {
                    Success = false,
                    Item = item,
                    Message = ex.Message
                });
            }
        }
        return results;
    }

    public async ValueTask DeleteAllAsync(IEnumerable<PageInfo> items)
    {
        ValidatePageInfos(items, "items");
        foreach (PageInfo item in items)
            await DeleteAsync(item.Id);
    }

    private static void ValidateId(int id, string parameterName) =>
        ThrowIf(id < 1, parameterName + " must be greater than 0.");

    private static void ValidatePageInfo(PageInfo pageInfo, string parameterName) =>
        ThrowIf(pageInfo == null, parameterName + " is required.");

    private static void ValidatePageInfos(IEnumerable<PageInfo> pageInfos, string parameterName) =>
        ThrowIf(pageInfos == null, parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
            throw new ValidationException(message);
    }
}
