// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.Data.Models.CMS;

using cCoder.ContentManagement.Exposures;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class PageInfoService(
    IPageInfoBroker pageInfoBroker,
    IPageBroker pageBroker,
    IAuthorizationManager authorizationManager) : IPageInfoService
{
    public PageInfo GetPageInfo(int pageInfoId, bool ignoreFilters = false) =>
        TryCatch<PageInfo>(operation: () =>
    {
        ValidatePageInfoOnGet(inputs: [pageInfoId, ignoreFilters]);
        ValidateId(pageInfoId: pageInfoId, parameterName: "id");

        if (ignoreFilters)
        {
            return ExecuteGetAllPageInfo(ignoreFilters: true)
                .FirstOrDefault(predicate: (PageInfo i) => i.Id == pageInfoId);
        }

        PageInfo pageInfo = ExecuteGetAllPageInfo()
            .FirstOrDefault(predicate: (PageInfo i) => i.Id == pageInfoId);

        if (pageInfo != null)
        {
            return pageInfo;
        }

        PageInfo pageInfo2 = ExecuteGetAllPageInfo(ignoreFilters: true)
            .FirstOrDefault(predicate: (PageInfo i) => i.Id == pageInfoId);

        if (pageInfo2 != null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return null;

    });

    public IQueryable<PageInfo> GetAllPageInfo(bool ignoreFilters = false) =>
        TryCatch<IQueryable<PageInfo>>(operation: () =>
    {
        ValidateAllPageInfoOnGet(inputs: [ignoreFilters]);

        return ignoreFilters
            ? pageInfoBroker.GetAllPageInfoIgnoringFilters()
            : pageInfoBroker.GetAllPageInfo();
    });

    public ValueTask<PageInfo> AddPageInfoAsync(PageInfo newPageInfo) =>
        TryCatch<PageInfo>(operation: async () =>
    {
        ValidatePageInfoOnAdd(inputs: [newPageInfo]);
        ValidatePageInfo(pageInfo: newPageInfo, parameterName: "pageInfo");
        authorizationManager.Authorize(appId: GetAppId(pageId: newPageInfo.PageId), privilege: "PageInfo_create");
        PageInfo result = await pageInfoBroker.AddPageInfoAsync(newPageInfo: CreateStoragePageInfo(newPageInfo: newPageInfo));
        newPageInfo.Id = result.Id;
        newPageInfo.PageId = result.PageId;
        newPageInfo.CultureId = result.CultureId;
        newPageInfo.Title = result.Title;
        newPageInfo.Description = result.Description;
        newPageInfo.Keywords = result.Keywords;
        return newPageInfo;

    }, isValueTask: true);

    public ValueTask<PageInfo> UpdatePageInfoAsync(PageInfo updatedPageInfo) =>
        TryCatch<PageInfo>(operation: async () =>
    {
        ValidatePageInfoOnUpdate(inputs: [updatedPageInfo]);
        ValidatePageInfo(pageInfo: updatedPageInfo, parameterName: "pageInfo");
        authorizationManager.Authorize(appId: GetAppId(pageId: updatedPageInfo.PageId), privilege: "PageInfo_update");
        PageInfo result = await pageInfoBroker.UpdatePageInfoAsync(updatedPageInfo: CreateStoragePageInfo(newPageInfo: updatedPageInfo));
        updatedPageInfo.Id = result.Id;
        updatedPageInfo.PageId = result.PageId;
        updatedPageInfo.CultureId = result.CultureId;
        updatedPageInfo.Title = result.Title;
        updatedPageInfo.Description = result.Description;
        updatedPageInfo.Keywords = result.Keywords;
        return updatedPageInfo;

    }, isValueTask: true);

    public ValueTask DeleteAsync(int pageInfoId) =>
        TryCatch(operation: async () =>
    {
        ValidateDeleteAsync(inputs: [pageInfoId]);
        ValidateId(pageInfoId: pageInfoId, parameterName: "id");
        PageInfo pageInfo;

        try
        {
            pageInfo = ExecuteGetPageInfo(pageInfoId: pageInfoId);
        }
        catch (SecurityException)
        {
            pageInfo = ExecuteGetPageInfo(pageInfoId: pageInfoId, ignoreFilters: true);
        }

        if (pageInfo == null)
        {
            return;
        }

        authorizationManager.Authorize(appId: GetAppId(pageId: pageInfo.PageId), privilege: "PageInfo_delete");
        await pageInfoBroker.DeletePageInfoAsync(deletedPageInfo: CreateStoragePageInfo(newPageInfo: pageInfo));

    }, isValueTask: true);

    private static PageInfo CreateStoragePageInfo(PageInfo newPageInfo)
    {
        if (newPageInfo == null)
        {
            return null;
        }

        return new PageInfo
        {
            Id = newPageInfo.Id,
            PageId = newPageInfo.PageId,
            CultureId = newPageInfo.CultureId,
            Title = newPageInfo.Title,
            Description = newPageInfo.Description,
            Keywords = newPageInfo.Keywords
        };
    }

    private int? GetAppId(int pageId) =>
        pageBroker.GetAllPagesIgnoringFilters()
        .Where(predicate: page => page.Id == pageId)
        .Select(selector: page => (int?)page.AppId)
        .FirstOrDefault();

    private IQueryable<PageInfo> ExecuteGetAllPageInfo(bool ignoreFilters = false) =>
        (ignoreFilters
            ? pageInfoBroker.GetAllPageInfoIgnoringFilters()
            : pageInfoBroker.GetAllPageInfo());

    private PageInfo ExecuteGetPageInfo(int pageInfoId, bool ignoreFilters = false)
    {
        ValidateId(pageInfoId: pageInfoId, parameterName: "id");

        if (ignoreFilters)
        {
            return ExecuteGetAllPageInfo(ignoreFilters: true)
                .FirstOrDefault(predicate: (PageInfo i) => i.Id == pageInfoId);
        }

        PageInfo pageInfo = ExecuteGetAllPageInfo()
            .FirstOrDefault(predicate: (PageInfo i) => i.Id == pageInfoId);

        if (pageInfo != null)
        {
            return pageInfo;
        }

        PageInfo pageInfo2 = ExecuteGetAllPageInfo(ignoreFilters: true)
            .FirstOrDefault(predicate: (PageInfo i) => i.Id == pageInfoId);

        if (pageInfo2 != null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return null;
    }
}