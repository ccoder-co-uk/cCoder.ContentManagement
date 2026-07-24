// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class PageService(IPageBroker pageBroker, IAuthorizationBroker authorizationBroker) : IPageService
{
    public Page GetPage(int pageId, bool ignoreFilters = false)
    {
        ValidateId(pageId: pageId, parameterName: "id");

        if (ignoreFilters)
        {
            return pageBroker.GetAllPages(ignoreFilters: true)
                .FirstOrDefault(predicate: page => page.Id == pageId);
        }

        Page result = pageBroker.GetAllPages(ignoreFilters: false)
            .FirstOrDefault(predicate: page => page.Id == pageId);

        if (result != null)
        {
            return result;
        }

        result = pageBroker.GetAllPages(ignoreFilters: true)
            .FirstOrDefault(predicate: foundPage => foundPage.Id == pageId);

        if (result != null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return null;
    }

    public IQueryable<Page> GetAllPage(bool ignoreFilters = false) =>
        pageBroker.GetAllPages(ignoreFilters: ignoreFilters);

    public async ValueTask<Page> AddPageAsync(Page page)
    {
        ValidatePage(page: page, parameterName: "page");
        authorizationBroker.Authorize(appId: page.AppId, privilege: "Page_create");
        Page newPage = CreateStoragePage(newPage: page);

        string currentUserId = authorizationBroker.GetCurrentUser()
            .Id;

        DateTimeOffset now = (newPage.CreatedOn = DateTimeOffset.UtcNow);
        newPage.CreatedBy = currentUserId;
        newPage.LastUpdated = now;
        newPage.LastUpdatedBy = currentUserId;
        Page result = await pageBroker.AddPageAsync(newPage: newPage);
        page.Id = result.Id;
        page.ParentId = result.ParentId;
        page.AppId = result.AppId;
        page.Order = result.Order;
        page.ShowOnMenus = result.ShowOnMenus;
        page.Name = result.Name;
        page.LastUpdated = result.LastUpdated;
        page.LastUpdatedBy = result.LastUpdatedBy;
        page.CreatedOn = result.CreatedOn;
        page.CreatedBy = result.CreatedBy;
        page.Path = result.Path;
        page.ResourceKey = result.ResourceKey;
        page.Layout = result.Layout;
        return page;
    }

    public async ValueTask<Page> UpdatePageAsync(Page updatedPage)
    {
        ValidatePage(page: updatedPage, parameterName: "page");
        authorizationBroker.Authorize(appId: updatedPage.AppId, privilege: "Page_update");
        Page updatePage = CreateStoragePage(newPage: updatedPage);

        string currentUserId = authorizationBroker.GetCurrentUser()
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
    }

    public async ValueTask DeleteAsync(int pageId)
    {
        ValidateId(pageId: pageId, parameterName: "id");
        Page page;

        try
        {
            page = GetPage(pageId: pageId);
        }
        catch (SecurityException)
        {
            page = GetPage(pageId: pageId, ignoreFilters: true);
        }

        if (page == null)
        {
            return;
        }

        authorizationBroker.Authorize(appId: page.AppId, privilege: "Page_delete");
        await pageBroker.DeletePageAsync(deletedPage: CreateStoragePage(newPage: page));
    }

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
}