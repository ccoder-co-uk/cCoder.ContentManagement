using System.Security;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using PageInfo = cCoder.Data.Models.CMS.PageInfo;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class PageInfoService(
    IPageInfoBroker pageInfoBroker,
    IPageBroker pageBroker,
    IAuthorizationBroker authorizationBroker) : IPageInfoService
{
    public PageInfo Get(int id, bool ignoreFilters = false)
    {
        ValidateId(id, "id");
        if (ignoreFilters)
        {
            return GetAll(ignoreFilters: true).FirstOrDefault((PageInfo i) => i.Id == id);
        }

        PageInfo pageInfo = GetAll().FirstOrDefault((PageInfo i) => i.Id == id);
        if (pageInfo != null)
        {
            return pageInfo;
        }
        PageInfo pageInfo2 = GetAll(ignoreFilters: true).FirstOrDefault((PageInfo i) => i.Id == id);
        if (pageInfo2 != null)
        {
            throw new SecurityException("Access Denied!");
        }
        return null;
    }

    public IQueryable<PageInfo> GetAll(bool ignoreFilters = false)
    {
        return pageInfoBroker.GetAllPageInfo(ignoreFilters);
    }

    public async ValueTask<PageInfo> AddAsync(PageInfo pageInfo)
    {
        ValidatePageInfo(pageInfo, "pageInfo");
        authorizationBroker.Authorize(GetAppId(pageInfo.PageId), "PageInfo_create");
        PageInfo result = await pageInfoBroker.AddPageInfoAsync(CreateStoragePageInfo(pageInfo));
        result.Page = pageInfo.Page;
        result.Culture = pageInfo.Culture;
        return result;
    }

    public async ValueTask<PageInfo> UpdateAsync(PageInfo pageInfo)
    {
        ValidatePageInfo(pageInfo, "pageInfo");
        authorizationBroker.Authorize(GetAppId(pageInfo.PageId), "PageInfo_update");
        PageInfo result = await pageInfoBroker.UpdatePageInfoAsync(CreateStoragePageInfo(pageInfo));
        result.Page = pageInfo.Page;
        result.Culture = pageInfo.Culture;
        return result;
    }

    public async ValueTask DeleteAsync(int id)
    {
        ValidateId(id, "id");
        PageInfo pageInfo;
        try
        {
            pageInfo = Get(id);
        }
        catch (SecurityException)
        {
            pageInfo = Get(id, ignoreFilters: true);
        }

        if (pageInfo == null)
        {
            return;
        }

        authorizationBroker.Authorize(GetAppId(pageInfo.PageId), "PageInfo_delete");
        await pageInfoBroker.DeletePageInfoAsync(CreateStoragePageInfo(pageInfo));
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
            .Where(page => page.Id == pageId)
            .Select(page => (int?)page.AppId)
            .FirstOrDefault();
    }
}
