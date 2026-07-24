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
    public App GetApp(int appId, bool ignoreFilters = false)
    {
        ValidateId(appId: appId, parameterName: "id");

        if (ignoreFilters)
        {
            return appBroker.GetAllApps(ignoreFilters: true)
                .FirstOrDefault(predicate: app => app.Id == appId);
        }

        App app = appBroker.GetAllApps(ignoreFilters: false)
            .FirstOrDefault(predicate: foundApp => foundApp.Id == appId);

        if (app != null)
        {
            return app;
        }

        App app2 = appBroker.GetAllApps(ignoreFilters: true)
            .FirstOrDefault(predicate: foundApp => foundApp.Id == appId);

        if (app2 != null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return null;
    }

    public IQueryable<App> GetAllApp(bool ignoreFilters = false) =>
        appBroker.GetAllApps(ignoreFilters: ignoreFilters);

    public async ValueTask<App> AddAppAsync(App newApp)
    {
        ValidateApp(app: newApp, parameterName: "app");
        authorizationBroker.Authorize(appId: null, privilege: "App_create");
        App storedApp = CreateStorageApp(newApp: newApp);
        App result = await appBroker.AddAppAsync(newApp: storedApp);
        newApp.Id = result.Id;
        newApp.DefaultCultureId = result.DefaultCultureId;
        newApp.TenantId = result.TenantId;
        newApp.Name = result.Name;
        newApp.Domain = result.Domain;
        newApp.DefaultTheme = result.DefaultTheme;
        newApp.ConfigJson = result.ConfigJson;
        return newApp;
    }

    public async ValueTask<App> UpdateAppAsync(App updatedApp)
    {
        ValidateApp(app: updatedApp, parameterName: "app");
        authorizationBroker.Authorize(appId: updatedApp.Id, privilege: "App_update");
        App storedApp = CreateStorageApp(newApp: updatedApp);
        App result = await appBroker.UpdateAppAsync(updatedApp: storedApp);
        updatedApp.Id = result.Id;
        updatedApp.DefaultCultureId = result.DefaultCultureId;
        updatedApp.TenantId = result.TenantId;
        updatedApp.Name = result.Name;
        updatedApp.Domain = result.Domain;
        updatedApp.DefaultTheme = result.DefaultTheme;
        updatedApp.ConfigJson = result.ConfigJson;
        return updatedApp;
    }

    public async ValueTask UpdatePageOrderAsync(int appId, IEnumerable<Page> updatedPage)
    {
        ValidateId(appId: appId, parameterName: "id");
        ValidatePages(pages: updatedPage, parameterName: "pages");
        authorizationBroker.Authorize(appId: appId, privilege: "App_update");
        Dictionary<int, Page> incomingPagesById = updatedPage.ToDictionary(keySelector: page => page.Id);

        List<Page> existingPages = pageBroker.GetAllPages(ignoreFilters: true)
            .Where(predicate: page => page.AppId == appId)
            .ToList();

        foreach (Page existingPage in existingPages)
        {
            if (incomingPagesById.TryGetValue(key: existingPage.Id, value: out Page incomingPage))
            {
                existingPage.Order = incomingPage.Order;
                existingPage.ParentId = incomingPage.ParentId;
                await pageBroker.UpdatePageAsync(updatedPage: existingPage);
            }
        }
    }

    public async ValueTask DeleteAsync(int appId)
    {
        ValidateId(appId: appId, parameterName: "id");
        App app = GetAppForDelete(appId: appId);

        if (app == null)
        {
            return;
        }

        if (app.Roles?.Any() == true)
        {
            authorizationBroker.Authorize(appId: app.Id, privilege: "App_delete");
        }

        await appBroker.DeleteAppAggregateAsync(deletedApp: app);
    }

    private App GetAppForDelete(int appId) =>
        appBroker.GetAllApps(ignoreFilters: true)
        .Include(navigationPropertyPath: app => app.Roles)
        .ThenInclude(navigationPropertyPath: role => role.Users)
        .FirstOrDefault(predicate: app => app.Id == appId);

    private static App CreateStorageApp(App newApp)
    {
        if (newApp == null)
        {
            return null;
        }

        return new App
        {
            Id = newApp.Id,
            DefaultCultureId = newApp.DefaultCultureId,
            TenantId = newApp.TenantId,
            Name = newApp.Name,
            Domain = newApp.Domain,
            DefaultTheme = newApp.DefaultTheme,
            ConfigJson = newApp.ConfigJson,
        };
    }

}