using System.Security;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using Microsoft.EntityFrameworkCore;
using App = cCoder.Data.Models.CMS.App;
using Page = cCoder.Data.Models.CMS.Page;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class AppService(
    IAppBroker appBroker,
    IPageBroker pageBroker,
    IAuthorizationBroker authorizationBroker) : IAppService
{
    public App Get(int id, bool ignoreFilters = false)
    {
        ValidateId(id, "id");
        if (ignoreFilters)
        {
            return appBroker.GetAllApps(ignoreFilters: true)
                .FirstOrDefault(app => app.Id == id);
        }

        App app = appBroker.GetAllApps(ignoreFilters: false)
            .FirstOrDefault(foundApp => foundApp.Id == id);

        if (app != null)
        {
            return app;
        }

        App app2 = appBroker.GetAllApps(ignoreFilters: true)
            .FirstOrDefault(foundApp => foundApp.Id == id);

        if (app2 != null)
        {
            throw new SecurityException("Access Denied!");
        }

        return null;
    }

    public IQueryable<App> GetAll(bool ignoreFilters = false)
    {
        return appBroker.GetAllApps(ignoreFilters);
    }

    public async ValueTask<App> AddAsync(App app)
    {
        ValidateApp(app, "app");
        authorizationBroker.Authorize(null, "App_create");
        App storedApp = CreateStorageApp(app);
        App result = await appBroker.AddAppAsync(storedApp);
        CopyChildren(result, app);
        return result;
    }

    public async ValueTask<App> UpdateAsync(App app)
    {
        ValidateApp(app, "app");
        authorizationBroker.Authorize(app.Id, "App_update");
        App storedApp = CreateStorageApp(app);
        App result = await appBroker.UpdateAppAsync(storedApp);
        CopyChildren(result, app);
        return result;
    }

    public async ValueTask UpdatePageOrderAsync(int id, IEnumerable<Page> pages)
    {
        ValidateId(id, "id");
        ValidatePages(pages, "pages");
        authorizationBroker.Authorize(id, "App_update");
        Dictionary<int, Page> incomingPagesById = pages.ToDictionary(page => page.Id);
        List<Page> existingPages = pageBroker.GetAllPages(ignoreFilters: true)
            .Where(page => page.AppId == id)
            .ToList();

        foreach (Page existingPage in existingPages)
        {
            if (incomingPagesById.TryGetValue(existingPage.Id, out Page incomingPage))
            {
                existingPage.Order = incomingPage.Order;
                existingPage.ParentId = incomingPage.ParentId;
                await pageBroker.UpdatePageAsync(existingPage);
            }
        }
    }

    public async ValueTask DeleteAsync(int id)
    {
        ValidateId(id, "id");
        App app = GetAppForDelete(id);

        if (app == null)
        {
            return;
        }

        if (app.Roles?.Any() == true)
        {
            authorizationBroker.Authorize(app.Id, "App_delete");
        }

        await appBroker.DeleteAppAggregateAsync(app);
    }

    private App GetAppForDelete(int id) =>
        appBroker.GetAllApps(ignoreFilters: true)
            .Include(app => app.Roles)
            .ThenInclude(role => role.Users)
            .FirstOrDefault(app => app.Id == id);

    private static App CreateStorageApp(App app)
    {
        if (app == null)
        {
            return null;
        }

        return new App
        {
            Id = app.Id,
            DefaultCultureId = app.DefaultCultureId,
            TenantId = app.TenantId,
            Name = app.Name,
            Domain = app.Domain,
            DefaultTheme = app.DefaultTheme,
            ConfigJson = app.ConfigJson
        };
    }

    private static void CopyChildren(App target, App source)
    {
        if (target == null || source == null)
        {
            return;
        }

        target.Cultures = source.Cultures;
        target.Roles = source.Roles;
        target.Pages = source.Pages;
        target.Components = source.Components;
        target.Scripts = source.Scripts;
        target.Templates = source.Templates;
        target.Resources = source.Resources;
        target.Layouts = source.Layouts;
    }
}
