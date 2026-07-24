// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using Microsoft.EntityFrameworkCore;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class AppService(
    IAppBroker appBroker,
    IPageBroker pageBroker,
    IAuthorizationBroker authorizationBroker) : IAppService
{
    public App Get(int id, bool ignoreFilters = false)
    {
        ValidateId(id: id, parameterName: "id");

        if (ignoreFilters)
        {
            return appBroker.GetAllApps(ignoreFilters: true)
                .FirstOrDefault(predicate: app => app.Id == id);
        }

        App app = appBroker.GetAllApps(ignoreFilters: false)
            .FirstOrDefault(predicate: foundApp => foundApp.Id == id);

        if (app != null)
        {
            return app;
        }

        App app2 = appBroker.GetAllApps(ignoreFilters: true)
            .FirstOrDefault(predicate: foundApp => foundApp.Id == id);

        if (app2 != null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return null;
    }

    public IQueryable<App> GetAll(bool ignoreFilters = false) =>
        appBroker.GetAllApps(ignoreFilters: ignoreFilters);

    public async ValueTask<App> AddAsync(App app)
    {
        ValidateApp(app: app, parameterName: "app");
        authorizationBroker.Authorize(appId: null, privilege: "App_create");
        App storedApp = CreateStorageApp(app: app);
        App result = await appBroker.AddAppAsync(entity: storedApp);
        app.Id = result.Id;
        app.DefaultCultureId = result.DefaultCultureId;
        app.TenantId = result.TenantId;
        app.Name = result.Name;
        app.Domain = result.Domain;
        app.DefaultTheme = result.DefaultTheme;
        app.ConfigJson = result.ConfigJson;
        return app;
    }

    public async ValueTask<App> UpdateAsync(App app)
    {
        ValidateApp(app: app, parameterName: "app");
        authorizationBroker.Authorize(appId: app.Id, privilege: "App_update");
        App storedApp = CreateStorageApp(app: app);
        App result = await appBroker.UpdateAppAsync(entity: storedApp);
        app.Id = result.Id;
        app.DefaultCultureId = result.DefaultCultureId;
        app.TenantId = result.TenantId;
        app.Name = result.Name;
        app.Domain = result.Domain;
        app.DefaultTheme = result.DefaultTheme;
        app.ConfigJson = result.ConfigJson;
        return app;
    }

    public async ValueTask UpdatePageOrderAsync(int id, IEnumerable<Page> pages)
    {
        ValidateId(id: id, parameterName: "id");
        ValidatePages(pages: pages, parameterName: "pages");
        authorizationBroker.Authorize(appId: id, privilege: "App_update");
        Dictionary<int, Page> incomingPagesById = pages.ToDictionary(keySelector: page => page.Id);

        List<Page> existingPages = pageBroker.GetAllPages(ignoreFilters: true)
            .Where(predicate: page => page.AppId == id)
            .ToList();

        foreach (Page existingPage in existingPages)
        {
            if (incomingPagesById.TryGetValue(key: existingPage.Id, value: out Page incomingPage))
            {
                existingPage.Order = incomingPage.Order;
                existingPage.ParentId = incomingPage.ParentId;
                await pageBroker.UpdatePageAsync(entity: existingPage);
            }
        }
    }

    public async ValueTask DeleteAsync(int id)
    {
        ValidateId(id: id, parameterName: "id");
        App app = GetAppForDelete(id: id);

        if (app == null)
        {
            return;
        }

        if (app.Roles?.Any() == true)
        {
            authorizationBroker.Authorize(appId: app.Id, privilege: "App_delete");
        }

        await appBroker.DeleteAppAggregateAsync(entity: app);
    }

    private App GetAppForDelete(int id) =>
        appBroker.GetAllApps(ignoreFilters: true)
        .Include(navigationPropertyPath: app => app.Roles)
        .ThenInclude(navigationPropertyPath: role => role.Users)
        .FirstOrDefault(predicate: app => app.Id == id);

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
            ConfigJson = app.ConfigJson,
        };
    }

}