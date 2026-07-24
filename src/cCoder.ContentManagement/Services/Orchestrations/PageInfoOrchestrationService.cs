// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;
using System.Security;
using cCoder.ContentManagement.Services.Processings;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal class PageInfoOrchestrationService(IPageInfoProcessingService processingService, IPageInfoEventProcessingService eventService) : IPageInfoOrchestrationService
{
    public PageInfo Get(int id) =>
        processingService.Get(id: ValidateId(id: id, parameterName: "id"));

    public IQueryable<PageInfo> GetAll(bool ignoreFilters = false) =>
        processingService.GetAll(ignoreFilters: ignoreFilters);

    public async ValueTask<PageInfo> AddAsync(PageInfo entity)
    {
        ValidatePageInfo(pageInfo: entity, parameterName: "entity");
        PageInfo result = await processingService.AddAsync(entity: entity);
        await eventService.RaisePageInfoAddEventAsync(entity: result);
        return result;
    }

    public async ValueTask<PageInfo> UpdateAsync(PageInfo entity)
    {
        ValidatePageInfo(pageInfo: entity, parameterName: "entity");
        PageInfo result = await processingService.UpdateAsync(entity: entity);
        await eventService.RaisePageInfoUpdateEventAsync(entity: result);
        return result;
    }

    public async ValueTask DeleteAsync(int id)
    {
        ValidateId(id: id, parameterName: "id");

        PageInfo entity;

        try
        {
            entity = processingService.Get(id: id);
        }
        catch (SecurityException)
        {
            entity = processingService.GetAll(ignoreFilters: true)
                .FirstOrDefault(predicate: pageInfo => pageInfo.Id == id);
        }

        if (entity == null)
        {
            return;
        }

        await eventService.RaisePageInfoDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(id: id);
    }

    public async ValueTask<IEnumerable<Result<PageInfo>>> AddOrUpdate(IEnumerable<PageInfo> items)
    {
        PageInfo[] pageInfos = ValidatePageInfos(pageInfos: items, parameterName: "items")
            .ToArray();

        List<Result<PageInfo>> results = new();

        foreach (PageInfo pageInfo in pageInfos)
        {
            try
            {
                PageInfo result = pageInfo.Id <= 0
                    ? await AddAsync(entity: pageInfo)
                    : await UpdateAsync(entity: pageInfo);

                results.Add(item: new Result<PageInfo>
                {
                    Success = true,
                    Item = result,
                    Message = pageInfo.Id <= 0 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new Result<PageInfo>
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
        PageInfo[] pageInfos = ValidatePageInfos(pageInfos: items, parameterName: "items")
            .ToArray();

        foreach (PageInfo pageInfo in pageInfos)
        {
            await DeleteAsync(id: pageInfo.Id);
        }
    }

    private static int ValidateId(int id, string parameterName)
    {
        if (id < 1)
        {
            throw new ValidationException(message: parameterName + " must be greater than 0.");
        }

        return id;
    }

    private static PageInfo ValidatePageInfo(PageInfo pageInfo, string parameterName)
    {
        if (pageInfo == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return pageInfo;
    }

    private static IEnumerable<PageInfo> ValidatePageInfos(IEnumerable<PageInfo> pageInfos, string parameterName)
    {
        if (pageInfos == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return pageInfos;
    }
}