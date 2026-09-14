// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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

    public App GetAppForDelete(int appId) =>
        TryCatch<App>(operation: () =>
    {
        ValidateAppForDeleteOnGet(inputs: [appId]);
        ValidateId(appId: appId, parameterName: "id");
        App app = processingService.GetAppForDelete(appId: appId);

        if (app?.Roles?.Any() == true)
        {
            Authorize(appId: appId, privilege: "app_delete");
        }

        return app;
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
        Authorize(appId: null, privilege: "app_create");

        return await processingService.AddAppAsync(newApp: newApp);
    }, isValueTask: true);

    public ValueTask<App> UpdateAppAsync(App updatedApp) =>
        TryCatch<App>(operation: async () =>
    {
        ValidateAppOnUpdate(inputs: [updatedApp]);
        ValidateApp(app: updatedApp, parameterName: "entity");
        Authorize(appId: updatedApp.Id, privilege: "app_update");

        return await processingService.UpdateAppAsync(updatedApp: updatedApp);
    }, isValueTask: true);

    public ValueTask RaiseAppAddEventAsync(App app) =>
        TryCatch(operation: () =>
    {
        ValidateAppEventOnRaise(inputs: [app]);
        ValidateApp(app: app, parameterName: "app");

        return eventProcessingService.RaiseAppAddEventAsync(
            app: app,
            userId: authorizationProcessingService.GetCurrentUserId());
    }, isValueTask: true);

    public ValueTask RaiseAppUpdateEventAsync(App app) =>
        TryCatch(operation: () =>
    {
        ValidateAppEventOnRaise(inputs: [app]);
        ValidateApp(app: app, parameterName: "app");

        return eventProcessingService.RaiseAppUpdateEventAsync(
            app: app,
            userId: authorizationProcessingService.GetCurrentUserId());
    }, isValueTask: true);

    public ValueTask RaiseAppDeleteEventAsync(App app) =>
        TryCatch(operation: () =>
    {
        ValidateAppEventOnRaise(inputs: [app]);
        ValidateApp(app: app, parameterName: "app");

        return eventProcessingService.RaiseAppDeleteEventAsync(
            app: app,
            userId: authorizationProcessingService.GetCurrentUserId());
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