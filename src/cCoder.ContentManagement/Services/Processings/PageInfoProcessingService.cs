using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Storages;
using PageInfo = cCoder.Data.Models.CMS.PageInfo;

namespace cCoder.ContentManagement.Services.Processings;

internal class PageInfoProcessingService(IPageInfoService service) : IPageInfoProcessingService
{
    public PageInfo Get(int id)
    {
        ValidateId(id, "id");
        return service.Get(id);
    }

    public IQueryable<PageInfo> GetAll(bool ignoreFilters = false)
    {
        return service.GetAll(ignoreFilters);
    }

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

    public async ValueTask<IEnumerable<cCoder.ContentManagement.Models.Result<PageInfo>>> AddOrUpdate(IEnumerable<PageInfo> items)
    {
        ValidatePageInfos(items, "items");
        List<cCoder.ContentManagement.Models.Result<PageInfo>> results = new List<cCoder.ContentManagement.Models.Result<PageInfo>>();
        foreach (PageInfo item in items)
        {
            try
            {
                PageInfo savedItem = item.Id < 1 ? await AddAsync(item) : await UpdateAsync(item);
                results.Add(new cCoder.ContentManagement.Models.Result<PageInfo>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id < 1 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(new cCoder.ContentManagement.Models.Result<PageInfo>
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
        {
            await DeleteAsync(item.Id);
        }
    }

    private static void ValidateId(int id, string parameterName)
    {
        if (id < 1)
        {
            throw new ValidationException(parameterName + " must be greater than 0.");
        }
    }

    private static void ValidatePageInfo(PageInfo pageInfo, string parameterName)
    {
        if (pageInfo == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
    }

    private static void ValidatePageInfos(IEnumerable<PageInfo> pageInfos, string parameterName)
    {
        if (pageInfos == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
    }
}
