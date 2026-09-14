// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using System.Security;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Coordinations;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Aggregations;

internal sealed partial class AppManagerAggregationService(
    IAppManagerCoordinationService appManagerCoordinationService,
    IAppLifecycleCoordinationService appLifecycleCoordinationService)
        : IAppManagerAggregationService
{
    public AppManagerContext GetAppManagerContext(
        AppManagerContext appManagerContext) =>
        TryCatch(operation: () =>
    {
        ValidateAppManagerContextOnGet(inputs: [appManagerContext]);
        ValidateAppManagerContext(appManagerContext: appManagerContext);

        appManagerContext.App = appManagerCoordinationService.GetApp(
            appId: appManagerContext.AppId);

        return appManagerContext;
    });

    public AppManagerContext GetByDomainAppManagerContext(
        AppManagerContext appManagerContext) =>
        TryCatch(operation: () =>
    {
        ValidateByDomainAppManagerContextOnGet(inputs: [appManagerContext]);
        ValidateAppManagerContext(appManagerContext: appManagerContext);

        appManagerContext.App = appManagerCoordinationService.GetByDomainApp(
            domain: appManagerContext.Domain,
            ignoreFilters: appManagerContext.IgnoreFilters);

        return appManagerContext;
    });

    public AppManagerContext GetAllAppManagerContext(
        AppManagerContext appManagerContext) =>
        TryCatch(operation: () =>
    {
        ValidateAllAppManagerContextOnGet(inputs: [appManagerContext]);
        ValidateAppManagerContext(appManagerContext: appManagerContext);

        appManagerContext.Apps = appManagerCoordinationService.GetAllApp(
            ignoreFilters: appManagerContext.IgnoreFilters);

        return appManagerContext;
    });

    public ValueTask<AppManagerContext> AddAppManagerContextAsync(
        AppManagerContext newAppManagerContext) =>
        TryCatch(operation: async () =>
    {
        ValidateAppManagerContextOnAdd(inputs: [newAppManagerContext]);
        ValidateAppManagerContext(appManagerContext: newAppManagerContext);

        newAppManagerContext.App = await appLifecycleCoordinationService
            .AddAppAsync(newApp: newAppManagerContext.App);

        return newAppManagerContext;
    }, isValueTask: true);

    public ValueTask<AppManagerContext> UpdateAppManagerContextAsync(
        AppManagerContext updatedAppManagerContext) =>
        TryCatch(operation: async () =>
    {
        ValidateAppManagerContextOnUpdate(inputs: [updatedAppManagerContext]);
        ValidateAppManagerContext(appManagerContext: updatedAppManagerContext);

        updatedAppManagerContext.App = await appLifecycleCoordinationService
            .UpdateAppAsync(updatedApp: updatedAppManagerContext.App);

        return updatedAppManagerContext;
    }, isValueTask: true);

    public ValueTask DeleteAppManagerContextAsync(
        AppManagerContext deletedAppManagerContext) =>
        TryCatch(operation: async () =>
    {
        ValidateAppManagerContextOnDelete(inputs: [deletedAppManagerContext]);
        ValidateAppManagerContext(appManagerContext: deletedAppManagerContext);

        await appLifecycleCoordinationService.DeleteAppAsync(
            appId: deletedAppManagerContext.AppId);
    }, isValueTask: true);

    public AppManagerContext GetAdminAppManagerContext(
        AppManagerContext appManagerContext) =>
        TryCatch(operation: () =>
    {
        ValidateAdminAppManagerContextOnGet(inputs: [appManagerContext]);
        ValidateAppManagerContext(appManagerContext: appManagerContext);

        appManagerContext.IsAdmin = appManagerCoordinationService.IsAdminApp(
            appId: appManagerContext.AppId,
            userName: appManagerContext.UserName);

        return appManagerContext;
    });

    public AppManagerContext GetUsersAppManagerContext(
        AppManagerContext appManagerContext) =>
        TryCatch(operation: () =>
    {
        ValidateUsersAppManagerContextOnGet(inputs: [appManagerContext]);
        ValidateAppManagerContext(appManagerContext: appManagerContext);

        App app = appManagerCoordinationService.GetAppWithUsers(
            appId: appManagerContext.AppId);

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
        ValidatePageOrderAppManagerContextOnUpdate(inputs: [updatedAppManagerContext]);
        ValidateAppManagerContext(appManagerContext: updatedAppManagerContext);

        await appManagerCoordinationService.UpdatePageOrderAppAsync(
            appId: updatedAppManagerContext.AppId,
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