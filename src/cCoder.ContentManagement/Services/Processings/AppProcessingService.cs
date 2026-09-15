// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using System.Security;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class AppProcessingService(
    IAppService service) : IAppProcessingService
{
    public ValueTask<App> GetAppForRenderAsync(int appId) =>
        TryCatch<App>(operation: async () =>
    {
        ValidateAppForRenderOnGet(inputs: [appId]);
        ValidateId(appId: appId, parameterName: "id");

        return (await service.GetAppForRenderAppOperationAsync(
            appOperation: new AppOperation { AppId = appId })).App;
    }, isValueTask: true);

    public App GetAppForDelete(int appId) =>
        TryCatch<App>(operation: () =>
    {
        ValidateAppForDeleteOnGet(inputs: [appId]);
        ValidateId(appId: appId, parameterName: "id");

        return service.GetAppForDeleteAppOperation(
            appOperation: new AppOperation { AppId = appId }).App;
    });

    public App GetApp(int appId) =>
        TryCatch<App>(operation: () =>
    {
        ValidateAppOnGet(inputs: [appId]);
        ValidateId(appId: appId, parameterName: "id");
        return GetAppFromStorage(appId: appId);
    });

    public string GetDomain(int appId, bool ignoreFilters = false) =>
        TryCatch<string>(operation: () =>
    {
        ValidateDomainOnGet(inputs: [appId, ignoreFilters]);
        ValidateId(appId: appId, parameterName: "id");

        return GetAllAppsFromStorage(ignoreFilters: ignoreFilters)
            .Where(predicate: app => app.Id == appId)
            .Select(selector: app => app.Domain)
            .FirstOrDefault();
    });

    public App GetByDomainApp(string domain, bool ignoreFilters = false) =>
        TryCatch<App>(operation: () =>
    {
        ValidateByDomainAppOnGet(inputs: [domain, ignoreFilters]);
        ValidateDomain(domain: domain, parameterName: "domain");

        return GetAllAppsFromStorage(ignoreFilters: ignoreFilters)
            .FirstOrDefault(predicate: app => app.Domain == domain);
    });

    public IQueryable<App> GetAllApp(bool ignoreFilters = false) =>
        TryCatch<IQueryable<App>>(operation: () =>
    {
        ValidateAllAppOnGet(inputs: [ignoreFilters]);
        return GetAllAppsFromStorage(ignoreFilters: ignoreFilters);
    });

    public ValueTask<App> AddAppAsync(App newApp) =>
        TryCatch<App>(operation: async () =>
    {
        ValidateAppOnAdd(inputs: [newApp]);
        ValidateApp(app: newApp, parameterName: "inputApp");

        return (await service.AddAppOperationAsync(
            newAppOperation: new AppOperation { App = newApp })).App;
    }, isValueTask: true);

    public ValueTask<App> UpdateAppAsync(App updatedApp) =>
        TryCatch<App>(operation: async () =>
    {
        ValidateAppOnUpdate(inputs: [updatedApp]);
        ValidateApp(app: updatedApp, parameterName: "app");
        App existingApp = GetAppFromStorage(appId: updatedApp.Id, ignoreFilters: true);

        if (existingApp == null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        existingApp.DefaultCultureId = updatedApp.DefaultCultureId;
        existingApp.TenantId = updatedApp.TenantId;
        existingApp.Name = updatedApp.Name;
        existingApp.Domain = updatedApp.Domain;
        existingApp.DefaultTheme = updatedApp.DefaultTheme;
        existingApp.ConfigJson = updatedApp.ConfigJson;

        return (await service.UpdateAppOperationAsync(
            updatedAppOperation: new AppOperation { App = existingApp })).App;
    }, isValueTask: true);

    public ValueTask DeleteAsync(int appId) =>
        TryCatch(operation: async () =>
    {
        ValidateDeleteAsync(inputs: [appId]);
        ValidateId(appId: appId, parameterName: "id");

        App app = service.GetAppForDeleteAppOperation(
            appOperation: new AppOperation { AppId = appId }).App;

        if (app != null)
        {
            await service.DeleteAppOperationAsync(
                deletedAppOperation: new AppOperation { App = app });
        }
    }, isValueTask: true);

    public ValueTask DeleteAllAppAsync(IEnumerable<App> deletedApp) =>
        TryCatch(operation: async () =>
    {
        ValidateAllAppOnDelete(inputs: [deletedApp]);
        ArgumentNullException.ThrowIfNull(argument: deletedApp);

        foreach (App app in deletedApp)
        {
            App storedApp = service.GetAppForDeleteAppOperation(
                appOperation: new AppOperation { AppId = app.Id }).App;

            if (storedApp != null)
            {
                await service.DeleteAppOperationAsync(
                    deletedAppOperation: new AppOperation { App = storedApp });
            }
        }
    }, isValueTask: true);

    private App GetAppFromStorage(int appId, bool ignoreFilters = false)
    {
        AppOperation appOperation = new() { AppId = appId };

        App app = ignoreFilters
            ? service.GetUnfilteredAppAppOperation(appOperation: appOperation).App
            : service.GetVisibleAppAppOperation(appOperation: appOperation).App;

        if (app != null || ignoreFilters)
        {
            return app;
        }

        bool exists = service.GetUnfilteredAppAppOperation(
            appOperation: new AppOperation { AppId = appId }).App != null;

        return exists
            ? throw new SecurityException(message: "Access Denied!")
            : null;
    }

    private IQueryable<App> GetAllAppsFromStorage(bool ignoreFilters = false)
    {
        AppOperation appOperation = new();

        return ignoreFilters
            ? service.GetUnfilteredAppsAppOperation(appOperation: appOperation).Apps
            : service.GetVisibleAppsAppOperation(appOperation: appOperation).Apps;
    }

    private static void ValidateId(int appId, string parameterName) =>
        ThrowIf(condition: appId < 1, message: parameterName + " must be greater than 0.");

    private static void ValidateApp(App app, string parameterName)
    {
        if (app == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        if (string.IsNullOrWhiteSpace(value: app.Name))
        {
            throw new ValidationException(message: parameterName + ".Name is required.");
        }

        if (string.IsNullOrWhiteSpace(value: app.Domain))
        {
            throw new ValidationException(message: parameterName + ".Domain is required.");
        }
    }

    private static void ValidateDomain(string domain, string parameterName) =>
        ThrowIf(
            condition: string.IsNullOrWhiteSpace(value: domain),
            message: parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }
}