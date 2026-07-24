// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Coordinations;

internal class AppRenderableCoordinationService(
    IPageOrchestrationService pageOrchestrationService,
    IComponentOrchestrationService componentOrchestrationService,
    ITemplateOrchestrationService templateOrchestrationService,
    ILayoutOrchestrationService layoutOrchestrationService) : IAppRenderableCoordinationService
{
    public async ValueTask HandleAppAddAsync(App app)
    {
        ValidateApp(app: app, parameterName: "app");
        StampChildrenWithApp(app: app);

        if (app.Templates != null)
        {
            await templateOrchestrationService.AddOrUpdateTemplateResult(newTemplate: app.Templates);
        }

        if (app.Layouts != null)
        {
            await layoutOrchestrationService.AddOrUpdateLayoutResult(newLayout: app.Layouts);
        }

        if (app.Components != null)
        {
            await AddOrUpdateComponentsAsync(newApp: app);
        }

        if (app.Pages != null)
        {
            await pageOrchestrationService.AddOrUpdatePageResult(newPage: app.Pages);
        }
    }

    public async ValueTask HandleAppUpdateAsync(App app)
    {
        ValidateApp(app: app, parameterName: "app");
        StampChildrenWithApp(app: app);

        if (app.Templates != null)
        {
            await DeleteMissingTemplatesAsync(deletedApp: app);
            await templateOrchestrationService.AddOrUpdateTemplateResult(newTemplate: app.Templates);
        }

        if (app.Layouts != null)
        {
            await DeleteMissingLayoutsAsync(deletedApp: app);
            await layoutOrchestrationService.AddOrUpdateLayoutResult(newLayout: app.Layouts);
        }

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
    }

    public async ValueTask HandleAppDeleteAsync(App app)
    {
        ValidateApp(app: app, parameterName: "app");
        await pageOrchestrationService.DeleteByAppIdAsync(appId: app.Id);
        await componentOrchestrationService.DeleteByAppIdAsync(appId: app.Id);
        await templateOrchestrationService.DeleteByAppIdAsync(appId: app.Id);
        await layoutOrchestrationService.DeleteByAppIdAsync(appId: app.Id);
    }

    private static void StampChildrenWithApp(App app)
    {
        if (app.Pages != null)
        {
            foreach (Page item in app.Pages)
            {
                item.AppId = app.Id;
            }
        }

        if (app.Components != null)
        {
            foreach (Component item2 in app.Components)
            {
                item2.AppId = app.Id;
            }
        }

        if (app.Templates != null)
        {
            foreach (Template item3 in app.Templates)
            {
                item3.AppId = app.Id;
            }
        }

        if (app.Layouts != null)
        {
            foreach (Layout item4 in app.Layouts)
            {
                item4.AppId = app.Id;
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
            .Where(predicate: page => page.AppId == deletedApp.Id && !((ReadOnlySpan<int>)incomingPageIds).Contains(value: page.Id))
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
            .Where(predicate: component => component.AppId == deletedApp.Id && !((ReadOnlySpan<int>)incomingComponentIds).Contains(value: component.Id))
            .ToArray();

        if (componentsToDelete.Length > 0)
        {
            await componentOrchestrationService.DeleteAllComponentAsync(deletedComponent: componentsToDelete);
        }
    }

    private async ValueTask DeleteMissingTemplatesAsync(App deletedApp)
    {
        int[] incomingTemplateIds = deletedApp.Templates
            .Where(predicate: template => template.Id > 0)
            .Select(selector: template => template.Id)
            .ToArray();

        Template[] templatesToDelete = templateOrchestrationService.GetAllTemplate(ignoreFilters: true)
            .Where(predicate: template => template.AppId == deletedApp.Id && !((ReadOnlySpan<int>)incomingTemplateIds).Contains(value: template.Id))
            .ToArray();

        if (templatesToDelete.Length > 0)
        {
            await templateOrchestrationService.DeleteAllTemplateAsync(deletedTemplate: templatesToDelete);
        }
    }

    private async ValueTask DeleteMissingLayoutsAsync(App deletedApp)
    {
        int[] incomingLayoutIds = deletedApp.Layouts
            .Where(predicate: layout => layout.Id > 0)
            .Select(selector: layout => layout.Id)
            .ToArray();

        Layout[] layoutsToDelete = layoutOrchestrationService.GetAllLayout(ignoreFilters: true)
            .Where(predicate: layout => layout.AppId == deletedApp.Id && !((ReadOnlySpan<int>)incomingLayoutIds).Contains(value: layout.Id))
            .ToArray();

        if (layoutsToDelete.Length > 0)
        {
            await layoutOrchestrationService.DeleteAllLayoutAsync(deletedLayout: layoutsToDelete);
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
                await componentOrchestrationService.UpdateComponentAsync(updatedComponent: component);
            }
            else
            {
                await componentOrchestrationService.AddComponentAsync(newComponent: component);
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