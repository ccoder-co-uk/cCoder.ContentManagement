// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;
using System.Security;
using cCoder.ContentManagement.Services.Processings;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal partial class PageInfoOrchestrationService(IPageInfoProcessingService processingService, IPageInfoEventProcessingService eventService) : IPageInfoOrchestrationService
{
    public PageInfo GetPageInfo(int pageInfoId) =>
        TryCatch<PageInfo>(operation: () =>
    {
        ValidatePageInfoOnGet(inputs: [pageInfoId]);
        return processingService.GetPageInfo(pageInfoId: ValidateId(pageInfoId: pageInfoId, parameterName: "id"));
    });

    public IQueryable<PageInfo> GetAllPageInfo(bool ignoreFilters = false) =>
        TryCatch<IQueryable<PageInfo>>(operation: () =>
    {
        ValidateAllPageInfoOnGet(inputs: [ignoreFilters]);
        return processingService.GetAllPageInfo(ignoreFilters: ignoreFilters);
    });

    public ValueTask<PageInfo> AddPageInfoAsync(PageInfo newPageInfo) =>
        TryCatch<PageInfo>(operation: async () =>
    {
        ValidatePageInfoOnAdd(inputs: [newPageInfo]);
        ValidatePageInfo(pageInfo: newPageInfo, parameterName: "entity");
        PageInfo result = await processingService.AddPageInfoAsync(newPageInfo: newPageInfo);
        await eventService.RaisePageInfoAddEventAsync(entity: result);
        return result;

    }, isValueTask: true);

    public ValueTask<PageInfo> UpdatePageInfoAsync(PageInfo updatedPageInfo) =>
        TryCatch<PageInfo>(operation: async () =>
    {
        ValidatePageInfoOnUpdate(inputs: [updatedPageInfo]);
        ValidatePageInfo(pageInfo: updatedPageInfo, parameterName: "entity");
        PageInfo result = await processingService.UpdatePageInfoAsync(updatedPageInfo: updatedPageInfo);
        await eventService.RaisePageInfoUpdateEventAsync(entity: result);
        return result;

    }, isValueTask: true);

    public ValueTask DeleteAsync(int pageInfoId) =>
        TryCatch(operation: async () =>
    {
        ValidateDeleteAsync(inputs: [pageInfoId]);
        ValidateId(pageInfoId: pageInfoId, parameterName: "id");

        PageInfo entity;

        try
        {
            entity = processingService.GetPageInfo(pageInfoId: pageInfoId);
        }
        catch (SecurityException)
        {
            entity = processingService.GetAllPageInfo(ignoreFilters: true)
                .FirstOrDefault(predicate: pageInfo => pageInfo.Id == pageInfoId);
        }

        if (entity == null)
        {
            return;
        }

        await eventService.RaisePageInfoDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(pageInfoId: pageInfoId);

    }, isValueTask: true);

    public ValueTask<IEnumerable<OperationResult<PageInfo>>> AddOrUpdatePageInfoResult(IEnumerable<PageInfo> newPageInfo) =>
        TryCatch<IEnumerable<OperationResult<PageInfo>>>(operation: async () =>
    {
        ValidateOrUpdatePageInfoResultOnAdd(inputs: [newPageInfo]);

        PageInfo[] pageInfos = ValidatePageInfos(pageInfos: newPageInfo, parameterName: "items")
            .ToArray();

        List<OperationResult<PageInfo>> results = new();

        foreach (PageInfo pageInfo in pageInfos)
        {
            try
            {
                PageInfo result = pageInfo.Id <= 0
                    ? await ExecuteAddPageInfoAsync(newPageInfo: pageInfo)
                    : await ExecuteUpdatePageInfoAsync(updatedPageInfo: pageInfo);

                results.Add(item: new OperationResult<PageInfo>
                {
                    Success = true,
                    Item = result,
                    Message = pageInfo.Id <= 0 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new OperationResult<PageInfo>
                {
                    Success = false,
                    Item = pageInfo,
                    Message = ex.Message
                });
            }
        }

        return results;

    }, isValueTask: true);

    public ValueTask DeleteAllPageInfoAsync(IEnumerable<PageInfo> deletedPageInfo) =>
        TryCatch(operation: async () =>
    {
        ValidateAllPageInfoOnDelete(inputs: [deletedPageInfo]);

        PageInfo[] pageInfos = ValidatePageInfos(pageInfos: deletedPageInfo, parameterName: "items")
            .ToArray();

        foreach (PageInfo pageInfo in pageInfos)
        {
            await ExecuteDeleteAsync(pageInfoId: pageInfo.Id);
        }

    }, isValueTask: true);

    private static int ValidateId(int pageInfoId, string parameterName)
    {
        if (pageInfoId < 1)
        {
            throw new ValidationException(message: parameterName + " must be greater than 0.");
        }

        return pageInfoId;
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

    private async ValueTask<PageInfo> ExecuteAddPageInfoAsync(PageInfo newPageInfo)
    {
        ValidatePageInfo(pageInfo: newPageInfo, parameterName: "entity");
        PageInfo result = await processingService.AddPageInfoAsync(newPageInfo: newPageInfo);
        await eventService.RaisePageInfoAddEventAsync(entity: result);
        return result;
    }

    private async ValueTask ExecuteDeleteAsync(int pageInfoId)
    {
        ValidateId(pageInfoId: pageInfoId, parameterName: "id");

        PageInfo entity;

        try
        {
            entity = processingService.GetPageInfo(pageInfoId: pageInfoId);
        }
        catch (SecurityException)
        {
            entity = processingService.GetAllPageInfo(ignoreFilters: true)
                .FirstOrDefault(predicate: pageInfo => pageInfo.Id == pageInfoId);
        }

        if (entity == null)
        {
            return;
        }

        await eventService.RaisePageInfoDeleteEventAsync(entity: entity);
        await processingService.DeleteAsync(pageInfoId: pageInfoId);
    }

    private async ValueTask<PageInfo> ExecuteUpdatePageInfoAsync(PageInfo updatedPageInfo)
    {
        ValidatePageInfo(pageInfo: updatedPageInfo, parameterName: "entity");
        PageInfo result = await processingService.UpdatePageInfoAsync(updatedPageInfo: updatedPageInfo);
        await eventService.RaisePageInfoUpdateEventAsync(entity: result);
        return result;
    }
}