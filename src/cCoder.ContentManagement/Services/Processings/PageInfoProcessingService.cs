// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class PageInfoProcessingService(IPageInfoService service) : IPageInfoProcessingService
{
    public PageInfo GetPageInfo(int pageInfoId) =>
        TryCatch<PageInfo>(operation: () =>
    {
        ValidatePageInfoOnGet(inputs: [pageInfoId]);
        ValidateId(pageInfoId: pageInfoId, parameterName: "id");
        return service.GetPageInfo(pageInfoId: pageInfoId);

    });

    public IQueryable<PageInfo> GetAllPageInfo(bool ignoreFilters = false) =>
        TryCatch<IQueryable<PageInfo>>(operation: () =>
    {
        ValidateAllPageInfoOnGet(inputs: [ignoreFilters]);
        return service.GetAllPageInfo(ignoreFilters: ignoreFilters);
    });

    public ValueTask<PageInfo> AddPageInfoAsync(PageInfo newPageInfo) =>
        TryCatch<PageInfo>(operation: () =>
    {
        ValidatePageInfoOnAdd(inputs: [newPageInfo]);
        ValidatePageInfo(pageInfo: newPageInfo, parameterName: "entity");
        NormalizeCulture(pageInfo: newPageInfo);
        return service.AddPageInfoAsync(newPageInfo: newPageInfo);

    }, isValueTask: true);

    public ValueTask<PageInfo> UpdatePageInfoAsync(PageInfo updatedPageInfo) =>
        TryCatch<PageInfo>(operation: () =>
    {
        ValidatePageInfoOnUpdate(inputs: [updatedPageInfo]);
        ValidatePageInfo(pageInfo: updatedPageInfo, parameterName: "entity");
        NormalizeCulture(pageInfo: updatedPageInfo);
        return service.UpdatePageInfoAsync(updatedPageInfo: updatedPageInfo);

    }, isValueTask: true);

    public ValueTask DeleteAsync(int pageInfoId) =>
        TryCatch(operation: () =>
    {
        ValidateDeleteAsync(inputs: [pageInfoId]);
        ValidateId(pageInfoId: pageInfoId, parameterName: "id");
        return service.DeleteAsync(pageInfoId: pageInfoId);

    }, isValueTask: true);

    public ValueTask<IEnumerable<OperationResult<PageInfo>>> AddOrUpdatePageInfoResult(IEnumerable<PageInfo> newPageInfo) =>
        TryCatch<IEnumerable<OperationResult<PageInfo>>>(operation: async () =>
    {
        ValidateOrUpdatePageInfoResultOnAdd(inputs: [newPageInfo]);
        ValidatePageInfos(pageInfos: newPageInfo, parameterName: "items");
        List<OperationResult<PageInfo>> results = new List<OperationResult<PageInfo>>();

        foreach (PageInfo item in newPageInfo)
        {
            try
            {
                PageInfo savedItem = item.Id < 1 ? await ExecuteAddPageInfoAsync(newPageInfo: item) : await ExecuteUpdatePageInfoAsync(updatedPageInfo: item);

                results.Add(item: new OperationResult<PageInfo>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id < 1 ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new OperationResult<PageInfo>
                {
                    Success = false,
                    Item = item,
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
        ValidatePageInfos(pageInfos: deletedPageInfo, parameterName: "items");

        foreach (PageInfo item in deletedPageInfo)
        {
            await ExecuteDeleteAsync(pageInfoId: item.Id);
        }

    }, isValueTask: true);

    private static void ValidateId(int pageInfoId, string parameterName) =>
        ThrowIf(condition: pageInfoId < 1, message: parameterName + " must be greater than 0.");

    private static void ValidatePageInfo(PageInfo pageInfo, string parameterName) =>
        ThrowIf(condition: pageInfo == null, message: parameterName + " is required.");

    private static void ValidatePageInfos(IEnumerable<PageInfo> pageInfos, string parameterName) =>
        ThrowIf(condition: pageInfos == null, message: parameterName + " is required.");

    private static void NormalizeCulture(PageInfo pageInfo) =>
        pageInfo.CultureId ??= string.Empty;

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
        NormalizeCulture(pageInfo: newPageInfo);
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
        NormalizeCulture(pageInfo: updatedPageInfo);
        return service.UpdatePageInfoAsync(updatedPageInfo: updatedPageInfo);
    }
}