// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using System.Security;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Aggregations;

internal sealed partial class AppManagerAggregationService(
    IAppOrchestrationService appOrchestrationService)
        : IAppManagerAggregationService
{
    public AppManagerContext GetAppManagerContext(
        AppManagerContext appManagerContext) =>
        TryCatch(operation: () =>
    {
        ValidateGetAppManagerContext(inputs: [appManagerContext]);
        ValidateAppManagerContext(appManagerContext: appManagerContext);

        appManagerContext.App = appOrchestrationService.GetApp(
            appId: appManagerContext.AppId);

        return appManagerContext;
    });

    public AppManagerContext GetByDomainAppManagerContext(
        AppManagerContext appManagerContext) =>
        TryCatch(operation: () =>
    {
        ValidateGetByDomainAppManagerContext(inputs: [appManagerContext]);
        ValidateAppManagerContext(appManagerContext: appManagerContext);

        appManagerContext.App = appOrchestrationService.GetByDomainApp(
            domain: appManagerContext.Domain,
            ignoreFilters: appManagerContext.IgnoreFilters);

        return appManagerContext;
    });

    public AppManagerContext GetAllAppManagerContext(
        AppManagerContext appManagerContext) =>
        TryCatch(operation: () =>
    {
        ValidateGetAllAppManagerContext(inputs: [appManagerContext]);
        ValidateAppManagerContext(appManagerContext: appManagerContext);

        appManagerContext.Apps = appOrchestrationService.GetAllApp(
            ignoreFilters: appManagerContext.IgnoreFilters);

        return appManagerContext;
    });

    public ValueTask<AppManagerContext> AddAppManagerContextAsync(
        AppManagerContext newAppManagerContext) =>
        TryCatch(operation: async () =>
    {
        ValidateAddAppManagerContextAsync(inputs: [newAppManagerContext]);
        ValidateAppManagerContext(appManagerContext: newAppManagerContext);

        newAppManagerContext.App = await appOrchestrationService.AddAppAsync(
            newApp: newAppManagerContext.App);

        return newAppManagerContext;
    }, isValueTask: true);

    public ValueTask<AppManagerContext> UpdateAppManagerContextAsync(
        AppManagerContext updatedAppManagerContext) =>
        TryCatch(operation: async () =>
    {
        ValidateUpdateAppManagerContextAsync(inputs: [updatedAppManagerContext]);
        ValidateAppManagerContext(appManagerContext: updatedAppManagerContext);

        updatedAppManagerContext.App = await appOrchestrationService.UpdateAppAsync(
            updatedApp: updatedAppManagerContext.App);

        return updatedAppManagerContext;
    }, isValueTask: true);

    public ValueTask DeleteAppManagerContextAsync(
        AppManagerContext deletedAppManagerContext) =>
        TryCatch(operation: async () =>
    {
        ValidateDeleteAppManagerContextAsync(inputs: [deletedAppManagerContext]);
        ValidateAppManagerContext(appManagerContext: deletedAppManagerContext);

        await appOrchestrationService.DeleteAsync(
            appId: deletedAppManagerContext.AppId);
    }, isValueTask: true);

    public AppManagerContext GetAdminAppManagerContext(
        AppManagerContext appManagerContext) =>
        TryCatch(operation: () =>
    {
        ValidateGetAdminAppManagerContext(inputs: [appManagerContext]);
        ValidateAppManagerContext(appManagerContext: appManagerContext);

        appManagerContext.IsAdmin = appOrchestrationService.IsAdminApp(
            appId: appManagerContext.AppId,
            userName: appManagerContext.UserName);

        return appManagerContext;
    });

    public AppManagerContext GetUsersAppManagerContext(
        AppManagerContext appManagerContext) =>
        TryCatch(operation: () =>
    {
        ValidateGetUsersAppManagerContext(inputs: [appManagerContext]);
        ValidateAppManagerContext(appManagerContext: appManagerContext);

        App app = appOrchestrationService.GetApp(
            appId: appManagerContext.AppId)
                ?? throw new SecurityException(message: "Access Denied!");

        appManagerContext.Users = app.Roles
            .SelectMany(selector: role => role.Users
                .Select(selector: userRole => userRole.User))
            .AsQueryable();

        return appManagerContext;
    });

    public ValueTask UpdatePageOrderAppManagerContextAsync(
        AppManagerContext updatedAppManagerContext) =>
        TryCatch(operation: async () =>
    {
        ValidateUpdatePageOrderAppManagerContextAsync(inputs: [updatedAppManagerContext]);
        ValidateAppManagerContext(appManagerContext: updatedAppManagerContext);

        await appOrchestrationService.UpdatePageOrderAppAsync(
            key: updatedAppManagerContext.AppId,
            updatedApp: updatedAppManagerContext.App);
    }, isValueTask: true);

    private static void ValidateAppManagerContext(
        AppManagerContext appManagerContext)
    {
        if (appManagerContext == null)
        {
            throw new ValidationException(message: "appManagerContext is required.");
        }
    }
}