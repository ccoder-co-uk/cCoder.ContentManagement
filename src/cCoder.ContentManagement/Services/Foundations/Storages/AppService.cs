// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Storages;
using cCoder.ContentManagement.Models;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class AppService(IAppBroker appBroker) : IAppService
{
    public AppOperation GetVisibleAppsAppOperation(AppOperation appOperation) =>
        TryCatch<AppOperation>(operation: () =>
    {
        ValidateVisibleAppsAppOperationOnGet(inputs: [appOperation]);
        appOperation.Apps = appBroker.GetAllApps();
        return appOperation;
    });

    public AppOperation GetUnfilteredAppsAppOperation(AppOperation appOperation) =>
        TryCatch<AppOperation>(operation: () =>
    {
        ValidateUnfilteredAppsAppOperationOnGet(inputs: [appOperation]);
        appOperation.Apps = appBroker.GetAllAppsIgnoringFilters();
        return appOperation;
    });

    public AppOperation GetVisibleAppAppOperation(AppOperation appOperation) =>
        TryCatch<AppOperation>(operation: () =>
    {
        ValidateVisibleAppAppOperationOnGet(inputs: [appOperation]);

        appOperation.App = appBroker.GetAllApps()
            .FirstOrDefault(predicate: app => app.Id == appOperation.AppId);

        return appOperation;
    });

    public AppOperation GetUnfilteredAppAppOperation(AppOperation appOperation) =>
        TryCatch<AppOperation>(operation: () =>
    {
        ValidateUnfilteredAppAppOperationOnGet(inputs: [appOperation]);

        appOperation.App = appBroker.GetAllAppsIgnoringFilters()
            .FirstOrDefault(predicate: app => app.Id == appOperation.AppId);

        return appOperation;
    });

    public ValueTask<AppOperation> GetAppForRenderAppOperationAsync(AppOperation appOperation) =>
        TryCatch<AppOperation>(operation: async () =>
    {
        ValidateAppForRenderAppOperationOnGet(inputs: [appOperation]);
        appOperation.App = await appBroker.GetAppForRenderAsync(appId: appOperation.AppId);
        return appOperation;
    }, isValueTask: true);

    public AppOperation GetAppForDeleteAppOperation(AppOperation appOperation) =>
        TryCatch<AppOperation>(operation: () =>
    {
        ValidateAppForDeleteAppOperationOnGet(inputs: [appOperation]);
        appOperation.App = appBroker.GetAppForDelete(appId: appOperation.AppId);
        return appOperation;
    });

    public ValueTask<AppOperation> AddAppOperationAsync(AppOperation newAppOperation) =>
        TryCatch<AppOperation>(operation: async () =>
    {
        ValidateAppOperationOnAdd(inputs: [newAppOperation]);
        App storageApp = CreateStorageApp(newApp: newAppOperation.App);
        App result = await appBroker.AddAppAsync(newApp: storageApp);
        CopyApp(source: result, destination: newAppOperation.App);
        return newAppOperation;
    }, isValueTask: true);

    public ValueTask<AppOperation> UpdateAppOperationAsync(AppOperation updatedAppOperation) =>
        TryCatch<AppOperation>(operation: async () =>
    {
        ValidateAppOperationOnUpdate(inputs: [updatedAppOperation]);
        App storageApp = CreateStorageApp(newApp: updatedAppOperation.App);
        App result = await appBroker.UpdateAppAsync(updatedApp: storageApp);
        CopyApp(source: result, destination: updatedAppOperation.App);
        return updatedAppOperation;
    }, isValueTask: true);

    public ValueTask<AppOperation> DeleteAppOperationAsync(AppOperation deletedAppOperation) =>
        TryCatch<AppOperation>(operation: async () =>
    {
        ValidateAppOperationOnDelete(inputs: [deletedAppOperation]);
        await appBroker.DeleteAppAggregateAsync(deletedApp: deletedAppOperation.App);
        return deletedAppOperation;
    }, isValueTask: true);

    private static App CreateStorageApp(App newApp) =>
        newApp == null ? null : new App
        {
            Id = newApp.Id,
            DefaultCultureId = newApp.DefaultCultureId,
            TenantId = newApp.TenantId,
            Name = newApp.Name,
            Domain = newApp.Domain,
            DefaultTheme = newApp.DefaultTheme,
            ConfigJson = newApp.ConfigJson,
        };

    private static void CopyApp(App source, App destination)
    {
        destination.Id = source.Id;
        destination.DefaultCultureId = source.DefaultCultureId;
        destination.TenantId = source.TenantId;
        destination.Name = source.Name;
        destination.Domain = source.Domain;
        destination.DefaultTheme = source.DefaultTheme;
        destination.ConfigJson = source.ConfigJson;
    }
}