// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;
using System.Security;

namespace cCoder.ContentManagement.Services.Coordinations;

internal sealed partial class AppManagerCoordinationService(
    IAppOrchestrationService appOrchestrationService,
    IAppPageOrderOrchestrationService appPageOrderOrchestrationService)
        : IAppManagerCoordinationService
{
    public App GetApp(int appId) =>
        TryCatch(operation: () =>
    {
        ValidateAppOnGet(inputs: [appId]);
        return appOrchestrationService.GetApp(appId: appId);
    });

    public App GetByDomainApp(string domain, bool ignoreFilters = false) =>
        TryCatch(operation: () =>
    {
        ValidateByDomainAppOnGet(inputs: [domain, ignoreFilters]);

        return appOrchestrationService.GetByDomainApp(
            domain: domain,
            ignoreFilters: ignoreFilters);
    });

    public IQueryable<App> GetAllApp(bool ignoreFilters = false) =>
        TryCatch(operation: () =>
    {
        ValidateAllAppOnGet(inputs: [ignoreFilters]);
        return appOrchestrationService.GetAllApp(ignoreFilters: ignoreFilters);
    });

    public bool IsAdminApp(int appId, string userName) =>
        TryCatch(operation: () =>
    {
        ValidateAdminAppOnGet(inputs: [appId, userName]);

        return appOrchestrationService.IsAdminApp(
            appId: appId,
            userName: userName);
    });

    public App GetAppWithUsers(int appId) =>
        TryCatch(operation: () =>
    {
        ValidateAppWithUsersOnGet(inputs: [appId]);

        return appOrchestrationService.GetApp(appId: appId)
            ?? throw new SecurityException(message: "Access Denied!");
    });

    public ValueTask UpdatePageOrderAppAsync(int appId, App updatedApp) =>
        TryCatch(operation: () =>
    {
        ValidatePageOrderAppOnUpdate(inputs: [appId, updatedApp]);

        return appPageOrderOrchestrationService.UpdatePageOrderAppAsync(
            appId: appId,
            updatedApp: updatedApp);
    }, isValueTask: true);
}