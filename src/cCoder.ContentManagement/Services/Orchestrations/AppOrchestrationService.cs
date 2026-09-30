// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal partial class AppOrchestrationService(
    IAppProcessingService processingService,
    IAuthorizationProcessingService authorizationProcessingService,
    IAppEventProcessingService eventProcessingService)
        : IAppOrchestrationService
{
    public ValueTask<App> GetAppForRenderAsync(int appId) =>
        TryCatch<App>(operation: async () =>
    {
        ValidateAppForRenderOnGet(inputs: [appId]);
        ValidateId(appId: appId, parameterName: "id");

        return await processingService.GetAppForRenderAsync(appId: appId);
    }, isValueTask: true);

    public App GetApp(int appId) =>
        TryCatch<App>(operation: () =>
    {
        ValidateAppOnGet(inputs: [appId]);
        ValidateId(appId: appId, parameterName: "id");
        return processingService.GetApp(appId: appId);
    });

    public bool IsAdminApp(int appId, string userName) =>
        TryCatch<bool>(operation: () =>
    {
        ValidateIsAdminApp(inputs: [appId, userName]);
        ValidateId(appId: appId, parameterName: "appId");
        ValidateUserName(userName: userName, parameterName: "userName");

        return authorizationProcessingService.IsAdminAuthorizationContext(
            context: new AuthorizationContext
            {
                Request = new AuthorizationRequest
                {
                    AppId = appId,
                    UserName = userName
                }
            });
    });

    public App GetByDomainApp(string domain, bool ignoreFilters = false) =>
        TryCatch<App>(operation: () =>
    {
        ValidateByDomainAppOnGet(inputs: [domain, ignoreFilters]);
        ValidateDomain(domain: domain, parameterName: "domain");

        return processingService.GetByDomainApp(
            domain: domain,
            ignoreFilters: ignoreFilters);
    });

    public IQueryable<App> GetAllApps(bool ignoreFilters = false) =>
        TryCatch<IQueryable<App>>(operation: () =>
    {
        ValidateAllAppsOnGet(inputs: [ignoreFilters]);
        return processingService.GetAllApps(ignoreFilters: ignoreFilters);
    });

    public ValueTask<App> AddAppAsync(App newApp) =>
        TryCatch<App>(operation: async () =>
    {
        ValidateAppOnAdd(inputs: [newApp]);
        ValidateApp(app: newApp, parameterName: "entity");
        Authorize(appId: null, privilege: "app_create");

        bool isFirstApp = !processingService
            .GetAllApps(ignoreFilters: true)
            .Any();

        processingService.PrepareNewApp(
            app: newApp,
            isFirstApp: isFirstApp);

        App storedApp = await processingService.AddAppAsync(newApp: newApp);

        processingService.StampAppChildren(app: storedApp);
        await processingService.PersistNewAppRolesAsync(app: storedApp);
        await ExecuteRaiseAppAddEventAsync(app: storedApp);

        return storedApp;
    }, isValueTask: true);

    public ValueTask<App> UpdateAppAsync(App updatedApp) =>
        TryCatch<App>(operation: async () =>
    {
        ValidateAppOnUpdate(inputs: [updatedApp]);
        ValidateApp(app: updatedApp, parameterName: "entity");
        Authorize(appId: updatedApp.Id, privilege: "app_update");

        App storedApp = await processingService.UpdateAppAsync(updatedApp: updatedApp);
        await ExecuteRaiseAppUpdateEventAsync(app: updatedApp);

        return storedApp;
    }, isValueTask: true);

    public ValueTask DeleteAppAsync(int appId) =>
        TryCatch(operation: async () =>
    {
        ValidateAppOnDelete(inputs: [appId]);
        ValidateId(appId: appId, parameterName: "appId");

        App app = ExecuteGetAppForDelete(appId: appId);

        if (app != null)
        {
            await ExecuteRaiseAppDeleteEventAsync(app: app);
        }
    }, isValueTask: true);

    public ValueTask HandleAppDeleteAsync(App app) =>
        TryCatch(operation: () =>
    {
        ValidateDeleteAsync(inputs: [app]);
        ValidateApp(app: app, parameterName: "app");
        return processingService.DeleteAsync(appId: app.Id);
    }, isValueTask: true);

    public ValueTask DeleteAllAppAsync(IEnumerable<App> deletedApp) =>
        TryCatch(operation: () =>
    {
        ValidateAllAppOnDelete(inputs: [deletedApp]);
        ArgumentNullException.ThrowIfNull(argument: deletedApp);
        return processingService.DeleteAllAppAsync(deletedApp: deletedApp);
    }, isValueTask: true);

    private App ExecuteGetAppForDelete(int appId)
    {
        App app = processingService.GetAppForDelete(appId: appId);

        if (app?.Roles?.Any() == true)
        {
            Authorize(appId: appId, privilege: "app_delete");
        }

        return app;
    }

    private ValueTask ExecuteRaiseAppAddEventAsync(App app) =>
        eventProcessingService.RaiseAppAddEventAsync(
            app: app,
            userId: authorizationProcessingService.GetCurrentUserId());

    private ValueTask ExecuteRaiseAppUpdateEventAsync(App app) =>
        eventProcessingService.RaiseAppUpdateEventAsync(
            app: app,
            userId: authorizationProcessingService.GetCurrentUserId());

    private ValueTask ExecuteRaiseAppDeleteEventAsync(App app) =>
        eventProcessingService.RaiseAppDeleteEventAsync(
            app: app,
            userId: authorizationProcessingService.GetCurrentUserId());

    private void Authorize(int? appId, string privilege) =>
        authorizationProcessingService.AuthorizeAuthorizationContext(
            context: new AuthorizationContext
            {
                Request = new AuthorizationRequest
                {
                    AppId = appId,
                    Privilege = privilege
                }
            });

    private static void ValidateId(int appId, string parameterName) =>
        ThrowIf(
            condition: appId < 1,
            message: parameterName + " must be greater than 0.");

    private static App ValidateApp(App app, string parameterName)
    {
        if (app == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return app;
    }

    private static void ValidateDomain(string domain, string parameterName) =>
        ThrowIf(
            condition: string.IsNullOrWhiteSpace(value: domain),
            message: parameterName + " is required.");

    private static void ValidateUserName(string userName, string parameterName) =>
        ThrowIf(
            condition: string.IsNullOrWhiteSpace(value: userName),
            message: parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }
}