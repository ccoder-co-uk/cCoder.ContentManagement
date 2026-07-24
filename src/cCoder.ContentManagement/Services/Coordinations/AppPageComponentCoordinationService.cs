// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Coordinations;

internal partial class AppPageComponentCoordinationService(
    IPageOrchestrationService pageOrchestrationService,
    IComponentOrchestrationService componentOrchestrationService)
        : IAppPageComponentCoordinationService
{
    public ValueTask HandleAppAddAsync(App app) =>
        TryCatch(operation: async () =>
    {
        ValidateHandleAppAddAsync(inputs: [app]);
        ValidateApp(app: app, parameterName: "app");
        StampChildrenWithApp(app: app);

        if (app.Components != null)
        {
            await AddOrUpdateComponentsAsync(newApp: app);
        }

        if (app.Pages != null)
        {
            await pageOrchestrationService.AddOrUpdatePageResult(newPage: app.Pages);
        }

    }, isValueTask: true);

    public ValueTask HandleAppUpdateAsync(App app) =>
        TryCatch(operation: async () =>
    {
        ValidateHandleAppUpdateAsync(inputs: [app]);
        ValidateApp(app: app, parameterName: "app");
        StampChildrenWithApp(app: app);

        if (app.Components != null)
        {
            await DeleteMissingComponentsAsync(deletedApp: app);
            await AddOrUpdateComponentsAsync(newApp: app);
        }

        if (app.Pages != null)
        {
            await DeleteMissingPagesAsync(deletedApp: app);
            await pageOrchestrationService.AddOrUpdatePageResult(newPage: app.Pages);
        }

    }, isValueTask: true);

    public ValueTask HandleAppDeleteAsync(App app) =>
        TryCatch(operation: async () =>
    {
        ValidateHandleAppDeleteAsync(inputs: [app]);
        ValidateApp(app: app, parameterName: "app");
        await pageOrchestrationService.DeleteByAppIdAsync(appId: app.Id);
        await componentOrchestrationService.DeleteByAppIdAsync(appId: app.Id);

    }, isValueTask: true);

    private static void StampChildrenWithApp(App app)
    {
        if (app.Pages != null)
        {
            foreach (Page page in app.Pages)
            {
                page.AppId = app.Id;
            }
        }

        if (app.Components != null)
        {
            foreach (Component component in app.Components)
            {
                component.AppId = app.Id;
            }
        }
    }

    private async ValueTask DeleteMissingPagesAsync(App deletedApp)
    {
        int[] incomingPageIds = deletedApp.Pages
            .Where(predicate: page => page.Id > 0)
            .Select(selector: page => page.Id)
            .ToArray();

        Page[] pagesToDelete = pageOrchestrationService.GetAllPage(ignoreFilters: true)
            .Where(predicate: page =>
                page.AppId == deletedApp.Id &&
                !((ReadOnlySpan<int>)incomingPageIds).Contains(value: page.Id))
            .ToArray();

        if (pagesToDelete.Length > 0)
        {
            await pageOrchestrationService.DeleteAllPageAsync(deletedPage: pagesToDelete);
        }
    }

    private async ValueTask DeleteMissingComponentsAsync(App deletedApp)
    {
        int[] incomingComponentIds = deletedApp.Components
            .Where(predicate: component => component.Id > 0)
            .Select(selector: component => component.Id)
            .ToArray();

        Component[] componentsToDelete = componentOrchestrationService.GetAllComponent(ignoreFilters: true)
            .Where(predicate: component =>
                component.AppId == deletedApp.Id &&
                !((ReadOnlySpan<int>)incomingComponentIds).Contains(value: component.Id))
            .ToArray();

        if (componentsToDelete.Length > 0)
        {
            await componentOrchestrationService.DeleteAllComponentAsync(
                deletedComponent: componentsToDelete);
        }
    }

    private async ValueTask AddOrUpdateComponentsAsync(App newApp)
    {
        HashSet<int> existingComponentIds = componentOrchestrationService.GetAllComponent(ignoreFilters: true)
            .Where(predicate: component => component.AppId == newApp.Id)
            .Select(selector: component => component.Id)
            .ToHashSet();

        foreach (Component component in newApp.Components)
        {
            if (existingComponentIds.Contains(item: component.Id))
            {
                await componentOrchestrationService.UpdateComponentAsync(
                    updatedComponent: component);
            }
            else
            {
                await componentOrchestrationService.AddComponentAsync(
                    newComponent: component);
            }
        }
    }

    private static void ValidateApp(App app, string parameterName) =>
        ThrowIf(condition: app == null, message: parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }
}