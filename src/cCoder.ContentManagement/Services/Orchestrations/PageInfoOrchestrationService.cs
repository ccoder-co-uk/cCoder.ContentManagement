using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Processings;
using PageInfo = cCoder.Data.Models.CMS.PageInfo;
using Result = cCoder.ContentManagement.Models.Result<cCoder.Data.Models.CMS.PageInfo>;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal class PageInfoOrchestrationService(IPageInfoProcessingService processingService, IPageInfoEventProcessingService eventService) : IPageInfoOrchestrationService
{
    public PageInfo Get(int id)
    {
        return processingService.Get(ValidateId(id, "id"));
    }

    public IQueryable<PageInfo> GetAll(bool ignoreFilters = false)
    {
        return processingService.GetAll(ignoreFilters);
    }

    public async ValueTask<PageInfo> AddAsync(PageInfo entity)
    {
        ValidatePageInfo(entity, "entity");
        PageInfo result = await processingService.AddAsync(entity);
        await eventService.RaisePageInfoAddEventAsync(result);
        return result;
    }

    public async ValueTask<PageInfo> UpdateAsync(PageInfo entity)
    {
        ValidatePageInfo(entity, "entity");
        PageInfo result = await processingService.UpdateAsync(entity);
        await eventService.RaisePageInfoUpdateEventAsync(result);
        return result;
    }

    public async ValueTask DeleteAsync(int id)
    {
        ValidateId(id, "id");
        PageInfo entity = processingService.Get(id);
        await eventService.RaisePageInfoDeleteEventAsync(entity);
        await processingService.DeleteAsync(id);
    }

    public async ValueTask<IEnumerable<Result>> AddOrUpdate(IEnumerable<PageInfo> items)
    {
        PageInfo[] pageInfos = ValidatePageInfos(items, "items").ToArray();
        List<Result> results = new();

        foreach (PageInfo pageInfo in pageInfos)
        {
            try
            {
                PageInfo result = pageInfo.Id <= 0
                    ? await AddAsync(pageInfo)
                    : await UpdateAsync(pageInfo);

                results.Add(new Result
                {
                    Success = true,
                    Item = result,
                    Message = pageInfo.Id <= 0 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(new Result
                {
                    Success = false,
                    Item = pageInfo,
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    public async ValueTask DeleteAllAsync(IEnumerable<PageInfo> items)
    {
        PageInfo[] pageInfos = ValidatePageInfos(items, "items").ToArray();

        foreach (PageInfo pageInfo in pageInfos)
        {
            await DeleteAsync(pageInfo.Id);
        }
    }

    private static int ValidateId(int id, string parameterName)
    {
        if (id < 1)
        {
            throw new ValidationException(parameterName + " must be greater than 0.");
        }
        return id;
    }

    private static PageInfo ValidatePageInfo(PageInfo pageInfo, string parameterName)
    {
        if (pageInfo == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
        return pageInfo;
    }

    private static IEnumerable<PageInfo> ValidatePageInfos(IEnumerable<PageInfo> pageInfos, string parameterName)
    {
        if (pageInfos == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
        return pageInfos;
    }
}
