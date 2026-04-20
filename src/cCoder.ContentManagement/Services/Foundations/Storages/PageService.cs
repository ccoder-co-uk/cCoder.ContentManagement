using System.Security;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using Page = cCoder.Data.Models.CMS.Page;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class PageService(IPageBroker pageBroker, IAuthorizationBroker authorizationBroker) : IPageService
{
    public Page Get(int id, bool ignoreFilters = false)
    {
        ValidateId(id, "id");
        if (ignoreFilters)
        {
            return pageBroker.GetAllPages(ignoreFilters: true)
                .FirstOrDefault(page => page.Id == id);
        }

        Page result = pageBroker.GetAllPages(ignoreFilters: false)
            .FirstOrDefault(page => page.Id == id);

        if (result != null)
        {
            return result;
        }

        result = pageBroker.GetAllPages(ignoreFilters: true)
            .FirstOrDefault(foundPage => foundPage.Id == id);

        if (result != null)
        {
            throw new SecurityException("Access Denied!");
        }

        return null;
    }

    public IQueryable<Page> GetAll(bool ignoreFilters = false)
    {
        return pageBroker.GetAllPages(ignoreFilters);
    }

    public async ValueTask<Page> AddAsync(Page page)
    {
        ValidatePage(page, "page");
        authorizationBroker.Authorize(page.AppId, "Page_create");
        Page newPage = CreateStoragePage(page);
        string currentUserId = authorizationBroker.GetCurrentUser().Id;
        DateTimeOffset now = (newPage.CreatedOn = DateTimeOffset.UtcNow);
        newPage.CreatedBy = currentUserId;
        newPage.LastUpdated = now;
        newPage.LastUpdatedBy = currentUserId;
        Page result = await pageBroker.AddPageAsync(newPage);
        result.App = page.App;
        result.Parent = page.Parent;
        result.PageInfo = page.PageInfo;
        result.Pages = page.Pages;
        result.Contents = page.Contents;
        result.Roles = page.Roles;
        return result;
    }

    public async ValueTask<Page> UpdateAsync(Page page)
    {
        ValidatePage(page, "page");
        authorizationBroker.Authorize(page.AppId, "Page_update");
        Page updatePage = CreateStoragePage(page);
        string currentUserId = authorizationBroker.GetCurrentUser().Id;
        DateTimeOffset now = DateTimeOffset.UtcNow;
        updatePage.LastUpdated = now;
        updatePage.LastUpdatedBy = currentUserId;
        Page result = await pageBroker.UpdatePageAsync(updatePage);
        result.App = page.App;
        result.Parent = page.Parent;
        result.PageInfo = page.PageInfo;
        result.Pages = page.Pages;
        result.Contents = page.Contents;
        result.Roles = page.Roles;
        return result;
    }

    public async ValueTask DeleteAsync(int id)
    {
        ValidateId(id, "id");
        Page page;
        try
        {
            page = Get(id);
        }
        catch (SecurityException)
        {
            page = Get(id, ignoreFilters: true);
        }

        if (page == null)
        {
            return;
        }

        authorizationBroker.Authorize(page.AppId, "Page_delete");
        await pageBroker.DeletePageAsync(CreateStoragePage(page));
    }

    private static Page CreateStoragePage(Page page)
    {
        if (page == null)
        {
            return null;
        }

        return new Page
        {
            Id = page.Id,
            ParentId = page.ParentId,
            AppId = page.AppId,
            Order = page.Order,
            ShowOnMenus = page.ShowOnMenus,
            Name = page.Name,
            LastUpdated = page.LastUpdated,
            LastUpdatedBy = page.LastUpdatedBy,
            CreatedOn = page.CreatedOn,
            CreatedBy = page.CreatedBy,
            Path = page.Path,
            ResourceKey = page.ResourceKey,
            Layout = page.Layout
        };
    }
}
