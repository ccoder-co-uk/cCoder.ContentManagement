// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using System.Security;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class PageInfoService(IPageInfoBroker pageInfoBroker) : IPageInfoService
{
    public PageInfo GetPageInfo(int pageInfoId, bool ignoreFilters = false) =>
        TryCatch<PageInfo>(operation: () =>
    {
        ValidatePageInfoOnGet(inputs: [pageInfoId, ignoreFilters]);
        ValidateId(pageInfoId: pageInfoId, parameterName: "id");

        if (ignoreFilters)
        {
            return ExecuteGetAllPageInfos(ignoreFilters: true)
                .FirstOrDefault(predicate: (PageInfo i) => i.Id == pageInfoId);
        }

        PageInfo pageInfo = ExecuteGetAllPageInfos()
            .FirstOrDefault(predicate: (PageInfo i) => i.Id == pageInfoId);

        if (pageInfo != null)
        {
            return pageInfo;
        }

        PageInfo pageInfo2 = ExecuteGetAllPageInfos(ignoreFilters: true)
            .FirstOrDefault(predicate: (PageInfo i) => i.Id == pageInfoId);

        if (pageInfo2 != null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return null;

    });

    public IQueryable<PageInfo> GetAllPageInfos(bool ignoreFilters = false) =>
        TryCatch<IQueryable<PageInfo>>(operation: () =>
    {
        ValidateAllPageInfosOnGet(inputs: [ignoreFilters]);

        return ignoreFilters
            ? pageInfoBroker.GetAllPageInfosIgnoringFilters()
            : pageInfoBroker.GetAllPageInfos();
    });

    public int? GetOwningAppId(int pageId) =>
        TryCatch<int?>(operation: () =>
    {
        ValidateOwningAppIdOnGet(inputs: [pageId]);
        ValidateId(pageInfoId: pageId, parameterName: "pageId");
        return pageInfoBroker.GetOwningAppId(pageId: pageId);
    });

    public ValueTask<PageInfo> AddPageInfoAsync(PageInfo newPageInfo) =>
        TryCatch<PageInfo>(operation: async () =>
    {
        ValidatePageInfoOnAdd(inputs: [newPageInfo]);
        ValidatePageInfo(pageInfo: newPageInfo, parameterName: "pageInfo");
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

    private IQueryable<PageInfo> ExecuteGetAllPageInfos(bool ignoreFilters = false) =>
        (ignoreFilters
            ? pageInfoBroker.GetAllPageInfosIgnoringFilters()
            : pageInfoBroker.GetAllPageInfos());

    private PageInfo ExecuteGetPageInfo(int pageInfoId, bool ignoreFilters = false)
    {
        ValidateId(pageInfoId: pageInfoId, parameterName: "id");

        if (ignoreFilters)
        {
            return ExecuteGetAllPageInfos(ignoreFilters: true)
                .FirstOrDefault(predicate: (PageInfo i) => i.Id == pageInfoId);
        }

        PageInfo pageInfo = ExecuteGetAllPageInfos()
            .FirstOrDefault(predicate: (PageInfo i) => i.Id == pageInfoId);

        if (pageInfo != null)
        {
            return pageInfo;
        }

        PageInfo pageInfo2 = ExecuteGetAllPageInfos(ignoreFilters: true)
            .FirstOrDefault(predicate: (PageInfo i) => i.Id == pageInfoId);

        if (pageInfo2 != null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return null;
    }
}