// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Brokers;
using Microsoft.EntityFrameworkCore;
using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal partial class AppOrchestrationService(
    IAppProcessingService processingService,
    IAppEventProcessingService eventService,
    IAuthorizationBroker authorizationBroker) : IAppOrchestrationService
{
    public App GetApp(int appId) =>
        TryCatch<App>(operation: () =>
    {
        ValidateAppOnGet(inputs: [appId]);
        ValidateId(appId: appId, parameterName: "id");
        return processingService.GetApp(appId: appId);

    });

    public App GetByDomainApp(string domain, bool ignoreFilters = false) =>
        TryCatch<App>(operation: () =>
    {
        ValidateByDomainAppOnGet(inputs: [domain, ignoreFilters]);
        ValidateDomain(domain: domain, parameterName: "domain");
        return processingService.GetByDomainApp(domain: domain, ignoreFilters: ignoreFilters);

    });

    public IQueryable<App> GetAllApp(bool ignoreFilters = false) =>
        TryCatch<IQueryable<App>>(operation: () =>
    {
        ValidateAllAppOnGet(inputs: [ignoreFilters]);
        return processingService.GetAllApp(ignoreFilters: ignoreFilters);
    });

    public ValueTask<App> AddAppAsync(App newApp) =>
        TryCatch<App>(operation: async () =>
    {
        ValidateAppOnAdd(inputs: [newApp]);
        ValidateApp(app: newApp, parameterName: "entity");
        App result = await processingService.AddAppAsync(newApp: newApp);
        await eventService.RaiseAppAddEventAsync(app: result);
        return result;

    }, isValueTask: true);

    public ValueTask<App> UpdateAppAsync(App updatedApp) =>
        TryCatch<App>(operation: async () =>
    {
        ValidateAppOnUpdate(inputs: [updatedApp]);
        ValidateApp(app: updatedApp, parameterName: "entity");
        App result = await processingService.UpdateAppAsync(updatedApp: updatedApp);
        ReflectUpdatedApp(source: result, target: updatedApp);
        await eventService.RaiseAppUpdateEventAsync(app: updatedApp);
        return result;

    }, isValueTask: true);

    public ValueTask DeleteAsync(int appId) =>
        TryCatch(operation: async () =>
    {
        ValidateDeleteAsync(inputs: [appId]);
        ValidateId(appId: appId, parameterName: "id");

        App app = processingService.GetAllApp(ignoreFilters: true)
            .Include(navigationPropertyPath: foundApp => foundApp.Roles)
            .FirstOrDefault(predicate: foundApp => foundApp.Id == appId);

        if (app?.Roles?.Any() == true)
        {
            authorizationBroker.Authorize(appId: appId, privilege: "app_delete");
        }

        if (app != null)
        {
            await eventService.RaiseAppDeleteEventAsync(app: app);
        }

        await processingService.DeleteAsync(appId: appId);

    }, isValueTask: true);

    public ValueTask<IEnumerable<Result<App>>> AddOrUpdateAppResult(IEnumerable<App> newApp) =>
        TryCatch<IEnumerable<Result<App>>>(operation: () =>
    {
        ValidateOrUpdateAppResultOnAdd(inputs: [newApp]);
        return processingService.AddOrUpdateAppResult(newApp: ValidateApps(apps: newApp, parameterName: "items"));
    }, isValueTask: true);

    public ValueTask DeleteAllAppAsync(IEnumerable<App> deletedApp) =>
        TryCatch(operation: () =>
    {
        ValidateAllAppOnDelete(inputs: [deletedApp]);
        return processingService.DeleteAllAppAsync(deletedApp: ValidateApps(apps: deletedApp, parameterName: "items"));
    }, isValueTask: true);

    public IQueryable<User> GetAppUsers(int appId) =>
        TryCatch<IQueryable<User>>(operation: () =>
    {
        ValidateAppUsersOnGet(inputs: [appId]);
        ValidateId(appId: appId, parameterName: "appId");
        return processingService.GetAppUsers(appId: appId);

    });

    public ValueTask UpdatePageOrderAppAsync(int key, App updatedApp) =>
        TryCatch(operation: () =>
    {
        ValidatePageOrderAppOnUpdate(inputs: [key, updatedApp]);
        return processingService.UpdatePageOrderAppAsync(key: key, updatedApp: ValidateApp(app: updatedApp, parameterName: "app"));
    }, isValueTask: true);

    public App ResolveCurrentApp() =>
        TryCatch<App>(operation: () =>
    {
        ValidateResolveCurrentApp(inputs: []);
        return processingService.ResolveCurrentApp();
    });

    private static void ValidateId(int appId, string parameterName) =>
        ThrowIf(condition: appId < 1, message: parameterName + " must be greater than 0.");

    private static App ValidateApp(App app, string parameterName)
    {
        if (app == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return app;
    }

    private static void ValidateDomain(string domain, string parameterName) =>
        ThrowIf(condition: string.IsNullOrWhiteSpace(value: domain), message: parameterName + " is required.");

    private static IEnumerable<App> ValidateApps(IEnumerable<App> apps, string parameterName)
    {
        if (apps == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return apps;
    }

    private static void ReflectUpdatedApp(App source, App target)
    {
        target.Id = source.Id;
        target.DefaultCultureId = source.DefaultCultureId;
        target.TenantId = source.TenantId;
        target.Name = source.Name;
        target.Domain = source.Domain;
        target.DefaultTheme = source.DefaultTheme;
        target.ConfigJson = source.ConfigJson;
        target.Cultures = source.Cultures;
        target.Pages = source.Pages;
        target.Components = source.Components;
        target.Scripts = source.Scripts;
        target.Roles = source.Roles;
        target.Templates = source.Templates;
        target.Resources = source.Resources;
        target.Layouts = source.Layouts;
    }

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }
}