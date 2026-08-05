// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.Data.Models.CMS;

using cCoder.ContentManagement.Exposures;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class PageService(IPageBroker pageBroker, IAuthorizationManager authorizationManager) : IPageService
{
    public ValueTask<Page> GetPageForRenderAsync(int pageId) =>
        TryCatch<Page>(operation: async () =>
    {
        ValidatePageForRenderOnGet(inputs: [pageId]);
        ValidateId(pageId: pageId, parameterName: "id");

        return await pageBroker.GetPageForRenderAsync(
            pageId: pageId);
    }, isValueTask: true);

    public Page GetPage(int pageId, bool ignoreFilters = false) =>
        TryCatch<Page>(operation: () =>
    {
        ValidatePageOnGet(inputs: [pageId, ignoreFilters]);
        ValidateId(pageId: pageId, parameterName: "id");

        if (ignoreFilters)
        {
            return pageBroker.GetAllPagesIgnoringFilters()
                .FirstOrDefault(predicate: page => page.Id == pageId);
        }

        Page result = pageBroker.GetAllPages()
            .FirstOrDefault(predicate: page => page.Id == pageId);

        if (result != null)
        {
            return result;
        }

        result = pageBroker.GetAllPagesIgnoringFilters()
            .FirstOrDefault(predicate: foundPage => foundPage.Id == pageId);

        if (result != null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return null;

    });

    public IQueryable<Page> GetAllPage(bool ignoreFilters = false) =>
        TryCatch<IQueryable<Page>>(operation: () =>
    {
        ValidateAllPageOnGet(inputs: [ignoreFilters]);

        return ignoreFilters
            ? pageBroker.GetAllPagesIgnoringFilters()
            : pageBroker.GetAllPages();
    });

    public ValueTask<Page> AddPageAsync(Page newPage) =>
        TryCatch<Page>(operation: async () =>
    {
        ValidatePageOnAdd(inputs: [newPage]);
        ValidatePage(page: newPage, parameterName: "page");
        authorizationManager.Authorize(appId: newPage.AppId, privilege: "Page_create");
        Page storagePage = CreateStoragePage(newPage: newPage);

        string currentUserId = authorizationManager.GetCurrentUser()
            .Id;

        DateTimeOffset now = (storagePage.CreatedOn = DateTimeOffset.UtcNow);
        storagePage.CreatedBy = currentUserId;
        storagePage.LastUpdated = now;
        storagePage.LastUpdatedBy = currentUserId;
        Page result = await pageBroker.AddPageAsync(newPage: storagePage);
        newPage.Id = result.Id;
        newPage.ParentId = result.ParentId;
        newPage.AppId = result.AppId;
        newPage.Order = result.Order;
        newPage.ShowOnMenus = result.ShowOnMenus;
        newPage.Name = result.Name;
        newPage.LastUpdated = result.LastUpdated;
        newPage.LastUpdatedBy = result.LastUpdatedBy;
        newPage.CreatedOn = result.CreatedOn;
        newPage.CreatedBy = result.CreatedBy;
        newPage.Path = result.Path;
        newPage.ResourceKey = result.ResourceKey;
        newPage.Layout = result.Layout;
        return newPage;

    }, isValueTask: true);

    public ValueTask<Page> UpdatePageAsync(Page updatedPage) =>
        TryCatch<Page>(operation: async () =>
    {
        ValidatePageOnUpdate(inputs: [updatedPage]);
        ValidatePage(page: updatedPage, parameterName: "page");
        authorizationManager.Authorize(appId: updatedPage.AppId, privilege: "Page_update");
        Page updatePage = CreateStoragePage(newPage: updatedPage);

        string currentUserId = authorizationManager.GetCurrentUser()
            .Id;

        DateTimeOffset now = DateTimeOffset.UtcNow;
        updatePage.LastUpdated = now;
        updatePage.LastUpdatedBy = currentUserId;
        Page result = await pageBroker.UpdatePageAsync(updatedPage: updatePage);
        updatedPage.Id = result.Id;
        updatedPage.ParentId = result.ParentId;
        updatedPage.AppId = result.AppId;
        updatedPage.Order = result.Order;
        updatedPage.ShowOnMenus = result.ShowOnMenus;
        updatedPage.Name = result.Name;
        updatedPage.LastUpdated = result.LastUpdated;
        updatedPage.LastUpdatedBy = result.LastUpdatedBy;
        updatedPage.CreatedOn = result.CreatedOn;
        updatedPage.CreatedBy = result.CreatedBy;
        updatedPage.Path = result.Path;
        updatedPage.ResourceKey = result.ResourceKey;
        updatedPage.Layout = result.Layout;
        return updatedPage;

    }, isValueTask: true);

    public ValueTask DeleteAsync(int pageId) =>
        TryCatch(operation: async () =>
    {
        ValidateDeleteAsync(inputs: [pageId]);
        ValidateId(pageId: pageId, parameterName: "id");
        Page page;

        try
        {
            page = ExecuteGetPage(pageId: pageId);
        }
        catch (SecurityException)
        {
            page = ExecuteGetPage(pageId: pageId, ignoreFilters: true);
        }

        if (page == null)
        {
            return;
        }

        authorizationManager.Authorize(appId: page.AppId, privilege: "Page_delete");
        await pageBroker.DeletePageAsync(deletedPage: CreateStoragePage(newPage: page));

    }, isValueTask: true);

    private static Page CreateStoragePage(Page newPage)
    {
        if (newPage == null)
        {
            return null;
        }

        return new Page
        {
            Id = newPage.Id,
            ParentId = newPage.ParentId,
            AppId = newPage.AppId,
            Order = newPage.Order,
            ShowOnMenus = newPage.ShowOnMenus,
            Name = newPage.Name,
            LastUpdated = newPage.LastUpdated,
            LastUpdatedBy = newPage.LastUpdatedBy,
            CreatedOn = newPage.CreatedOn,
            CreatedBy = newPage.CreatedBy,
            Path = newPage.Path,
            ResourceKey = newPage.ResourceKey,
            Layout = newPage.Layout
        };
    }

    private Page ExecuteGetPage(int pageId, bool ignoreFilters = false)
    {
        ValidateId(pageId: pageId, parameterName: "id");

        if (ignoreFilters)
        {
            return pageBroker.GetAllPagesIgnoringFilters()
                .FirstOrDefault(predicate: page => page.Id == pageId);
        }

        Page result = pageBroker.GetAllPages()
            .FirstOrDefault(predicate: page => page.Id == pageId);

        if (result != null)
        {
            return result;
        }

        result = pageBroker.GetAllPagesIgnoringFilters()
            .FirstOrDefault(predicate: foundPage => foundPage.Id == pageId);

        if (result != null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return null;
    }
}