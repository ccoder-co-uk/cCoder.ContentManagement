// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

internal class PageInfoProcessingService(IPageInfoService service) : IPageInfoProcessingService
{
    public PageInfo GetPageInfo(int pageInfoId)
    {
        ValidateId(pageInfoId: pageInfoId, parameterName: "id");
        return service.GetPageInfo(pageInfoId: pageInfoId);
    }

    public IQueryable<PageInfo> GetAllPageInfo(bool ignoreFilters = false) =>
        service.GetAllPageInfo(ignoreFilters: ignoreFilters);

    public ValueTask<PageInfo> AddPageInfoAsync(PageInfo newPageInfo)
    {
        ValidatePageInfo(pageInfo: newPageInfo, parameterName: "entity");
        return service.AddPageInfoAsync(newPageInfo: newPageInfo);
    }

    public ValueTask<PageInfo> UpdatePageInfoAsync(PageInfo updatedPageInfo)
    {
        ValidatePageInfo(pageInfo: updatedPageInfo, parameterName: "entity");
        return service.UpdatePageInfoAsync(updatedPageInfo: updatedPageInfo);
    }

    public ValueTask DeleteAsync(int pageInfoId)
    {
        ValidateId(pageInfoId: pageInfoId, parameterName: "id");
        return service.DeleteAsync(pageInfoId: pageInfoId);
    }

    public async ValueTask<IEnumerable<Result<PageInfo>>> AddOrUpdatePageInfoResult(IEnumerable<PageInfo> newPageInfo)
    {
        ValidatePageInfos(pageInfos: newPageInfo, parameterName: "items");
        List<Result<PageInfo>> results = new List<Result<PageInfo>>();

        foreach (PageInfo item in newPageInfo)
        {
            try
            {
                PageInfo savedItem = item.Id < 1 ? await ExecuteAddPageInfoAsync(newPageInfo: item) : await ExecuteUpdatePageInfoAsync(updatedPageInfo: item);

                results.Add(item: new Result<PageInfo>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id < 1 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new Result<PageInfo>
                {
                    Success = false,
                    Item = item,
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    public async ValueTask DeleteAllPageInfoAsync(IEnumerable<PageInfo> deletedPageInfo)
    {
        ValidatePageInfos(pageInfos: deletedPageInfo, parameterName: "items");

        foreach (PageInfo item in deletedPageInfo)
        {
            await ExecuteDeleteAsync(pageInfoId: item.Id);
        }
    }

    private static void ValidateId(int pageInfoId, string parameterName) =>
        ThrowIf(condition: pageInfoId < 1, message: parameterName + " must be greater than 0.");

    private static void ValidatePageInfo(PageInfo pageInfo, string parameterName) =>
        ThrowIf(condition: pageInfo == null, message: parameterName + " is required.");

    private static void ValidatePageInfos(IEnumerable<PageInfo> pageInfos, string parameterName) =>
        ThrowIf(condition: pageInfos == null, message: parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }

    private ValueTask<PageInfo> ExecuteAddPageInfoAsync(PageInfo newPageInfo)
    {
        ValidatePageInfo(pageInfo: newPageInfo, parameterName: "entity");
        return service.AddPageInfoAsync(newPageInfo: newPageInfo);
    }

    private ValueTask ExecuteDeleteAsync(int pageInfoId)
    {
        ValidateId(pageInfoId: pageInfoId, parameterName: "id");
        return service.DeleteAsync(pageInfoId: pageInfoId);
    }

    private ValueTask<PageInfo> ExecuteUpdatePageInfoAsync(PageInfo updatedPageInfo)
    {
        ValidatePageInfo(pageInfo: updatedPageInfo, parameterName: "entity");
        return service.UpdatePageInfoAsync(updatedPageInfo: updatedPageInfo);
    }
}