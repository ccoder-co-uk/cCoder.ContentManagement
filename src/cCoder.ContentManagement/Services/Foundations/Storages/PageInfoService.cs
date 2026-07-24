// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class PageInfoService(
    IPageInfoBroker pageInfoBroker,
    IPageBroker pageBroker,
    IAuthorizationBroker authorizationBroker) : IPageInfoService
{
    public PageInfo Get(int id, bool ignoreFilters = false)
    {
        ValidateId(id: id, parameterName: "id");

        if (ignoreFilters)
        {
            return GetAll(ignoreFilters: true)
                        .FirstOrDefault(predicate: (PageInfo i) => i.Id == id);
        }

        PageInfo pageInfo = GetAll()
            .FirstOrDefault(predicate: (PageInfo i) => i.Id == id);

        if (pageInfo != null)
        {
            return pageInfo;
        }

        PageInfo pageInfo2 = GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: (PageInfo i) => i.Id == id);

        if (pageInfo2 != null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return null;
    }

    public IQueryable<PageInfo> GetAll(bool ignoreFilters = false) =>
        pageInfoBroker.GetAllPageInfo(ignoreFilters: ignoreFilters);

    public async ValueTask<PageInfo> AddAsync(PageInfo pageInfo)
    {
        ValidatePageInfo(pageInfo: pageInfo, parameterName: "pageInfo");
        authorizationBroker.Authorize(appId: GetAppId(pageId: pageInfo.PageId), privilege: "PageInfo_create");
        PageInfo result = await pageInfoBroker.AddPageInfoAsync(entity: CreateStoragePageInfo(pageInfo: pageInfo));
        pageInfo.Id = result.Id;
        pageInfo.PageId = result.PageId;
        pageInfo.CultureId = result.CultureId;
        pageInfo.Title = result.Title;
        pageInfo.Description = result.Description;
        pageInfo.Keywords = result.Keywords;
        return pageInfo;
    }

    public async ValueTask<PageInfo> UpdateAsync(PageInfo pageInfo)
    {
        ValidatePageInfo(pageInfo: pageInfo, parameterName: "pageInfo");
        authorizationBroker.Authorize(appId: GetAppId(pageId: pageInfo.PageId), privilege: "PageInfo_update");
        PageInfo result = await pageInfoBroker.UpdatePageInfoAsync(entity: CreateStoragePageInfo(pageInfo: pageInfo));
        pageInfo.Id = result.Id;
        pageInfo.PageId = result.PageId;
        pageInfo.CultureId = result.CultureId;
        pageInfo.Title = result.Title;
        pageInfo.Description = result.Description;
        pageInfo.Keywords = result.Keywords;
        return pageInfo;
    }

    public async ValueTask DeleteAsync(int id)
    {
        ValidateId(id: id, parameterName: "id");
        PageInfo pageInfo;

        try
        {
            pageInfo = Get(id: id);
        }
        catch (SecurityException)
        {
            pageInfo = Get(id: id, ignoreFilters: true);
        }

        if (pageInfo == null)
        {
            return;
        }

        authorizationBroker.Authorize(appId: GetAppId(pageId: pageInfo.PageId), privilege: "PageInfo_delete");
        await pageInfoBroker.DeletePageInfoAsync(entity: CreateStoragePageInfo(pageInfo: pageInfo));
    }

    private static PageInfo CreateStoragePageInfo(PageInfo pageInfo)
    {
        if (pageInfo == null)
        {
            return null;
        }

        return new PageInfo
        {
            Id = pageInfo.Id,
            PageId = pageInfo.PageId,
            CultureId = pageInfo.CultureId,
            Title = pageInfo.Title,
            Description = pageInfo.Description,
            Keywords = pageInfo.Keywords
        };
    }

    private int? GetAppId(int pageId)
    {
        return pageBroker.GetAllPages(ignoreFilters: true)
            .Where(predicate: page => page.Id == pageId)
            .Select(selector: page => (int?)page.AppId)
            .FirstOrDefault();
    }
}