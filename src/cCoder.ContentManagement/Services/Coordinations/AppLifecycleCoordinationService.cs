// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Coordinations;

internal sealed partial class AppLifecycleCoordinationService(
    IAppOrchestrationService appOrchestrationService,
    IAppBootstrapOrchestrationService bootstrapOrchestrationService,
    IAppRoleOrchestrationService roleOrchestrationService)
        : IAppLifecycleCoordinationService
{
    public ValueTask<App> AddAppAsync(App newApp) =>
        TryCatch<App>(operation: async () =>
    {
        ValidateAppOnAdd(inputs: [newApp]);
        ArgumentNullException.ThrowIfNull(argument: newApp);

        bool isFirstApp = !appOrchestrationService
            .GetAllApp(ignoreFilters: true)
            .Any();

        bootstrapOrchestrationService.PrepareNewApp(
            app: newApp,
            isFirstApp: isFirstApp);

        App storedApp = await appOrchestrationService.AddAppAsync(
            newApp: newApp);

        bootstrapOrchestrationService.StampAppChildren(app: storedApp);

        await roleOrchestrationService.PersistNewAppRolesAsync(
            app: storedApp);

        await appOrchestrationService.RaiseAppAddEventAsync(app: storedApp);
        return storedApp;
    }, isValueTask: true);

    public ValueTask<App> UpdateAppAsync(App updatedApp) =>
        TryCatch<App>(operation: async () =>
    {
        ValidateAppOnUpdate(inputs: [updatedApp]);
        ArgumentNullException.ThrowIfNull(argument: updatedApp);

        App storedApp = await appOrchestrationService.UpdateAppAsync(
            updatedApp: updatedApp);

        await appOrchestrationService.RaiseAppUpdateEventAsync(app: updatedApp);
        return storedApp;
    }, isValueTask: true);

    public ValueTask DeleteAppAsync(int appId) =>
        TryCatch(operation: async () =>
    {
        ValidateAppOnDelete(inputs: [appId]);
        App app = appOrchestrationService.GetAppForDelete(appId: appId);

        if (app != null)
        {
            await appOrchestrationService.RaiseAppDeleteEventAsync(app: app);
        }
    }, isValueTask: true);
}