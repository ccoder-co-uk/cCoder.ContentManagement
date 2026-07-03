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
        ValidateApp(app, "app");
        StampChildrenWithApp(app);

        if (app.Templates != null)
            await templateOrchestrationService.AddOrUpdate(app.Templates);

        if (app.Layouts != null)
            await layoutOrchestrationService.AddOrUpdate(app.Layouts);

        if (app.Components != null)
            await AddOrUpdateComponentsAsync(app);

        if (app.Pages != null)
            await pageOrchestrationService.AddOrUpdate(app.Pages);
    }

    public async ValueTask HandleAppUpdateAsync(App app)
    {
        ValidateApp(app, "app");
        StampChildrenWithApp(app);

        if (app.Templates != null)
        {
            await DeleteMissingTemplatesAsync(app);
            await templateOrchestrationService.AddOrUpdate(app.Templates);
        }

        if (app.Layouts != null)
        {
            await DeleteMissingLayoutsAsync(app);
            await layoutOrchestrationService.AddOrUpdate(app.Layouts);
        }

        if (app.Components != null)
        {
            await DeleteMissingComponentsAsync(app);
            await AddOrUpdateComponentsAsync(app);
        }

        if (app.Pages != null)
        {
            await DeleteMissingPagesAsync(app);
            await pageOrchestrationService.AddOrUpdate(app.Pages);
        }
    }

    public async ValueTask HandleAppDeleteAsync(App app)
    {
        ValidateApp(app, "app");
        await pageOrchestrationService.DeleteByAppIdAsync(app.Id);
        await componentOrchestrationService.DeleteByAppIdAsync(app.Id);
        await templateOrchestrationService.DeleteByAppIdAsync(app.Id);
        await layoutOrchestrationService.DeleteByAppIdAsync(app.Id);
    }

    private static void StampChildrenWithApp(App app)
    {
        if (app.Pages != null)
            foreach (Page item in app.Pages)
                item.AppId = app.Id;

        if (app.Components != null)
            foreach (Component item2 in app.Components)
                item2.AppId = app.Id;

        if (app.Templates != null)
            foreach (Template item3 in app.Templates)
                item3.AppId = app.Id;

        if (app.Layouts != null)
            foreach (Layout item4 in app.Layouts)
                item4.AppId = app.Id;
    }

    private async ValueTask DeleteMissingPagesAsync(App app)
    {
        int[] incomingPageIds = app.Pages
            .Where(page => page.Id > 0)
            .Select(page => page.Id)
            .ToArray();

        Page[] pagesToDelete = pageOrchestrationService.GetAll(ignoreFilters: true)
            .Where(page => page.AppId == app.Id && !((ReadOnlySpan<int>)incomingPageIds).Contains(page.Id))
            .ToArray();

        if (pagesToDelete.Length > 0)
            await pageOrchestrationService.DeleteAllAsync(pagesToDelete);
    }

    private async ValueTask DeleteMissingComponentsAsync(App app)
    {
        int[] incomingComponentIds = app.Components
            .Where(component => component.Id > 0)
            .Select(component => component.Id)
            .ToArray();

        Component[] componentsToDelete = componentOrchestrationService.GetAll(ignoreFilters: true)
            .Where(component => component.AppId == app.Id && !((ReadOnlySpan<int>)incomingComponentIds).Contains(component.Id))
            .ToArray();

        if (componentsToDelete.Length > 0)
            await componentOrchestrationService.DeleteAllAsync(componentsToDelete);
    }

    private async ValueTask DeleteMissingTemplatesAsync(App app)
    {
        int[] incomingTemplateIds = app.Templates
            .Where(template => template.Id > 0)
            .Select(template => template.Id)
            .ToArray();

        Template[] templatesToDelete = templateOrchestrationService.GetAll(ignoreFilters: true)
            .Where(template => template.AppId == app.Id && !((ReadOnlySpan<int>)incomingTemplateIds).Contains(template.Id))
            .ToArray();

        if (templatesToDelete.Length > 0)
            await templateOrchestrationService.DeleteAllAsync(templatesToDelete);
    }

    private async ValueTask DeleteMissingLayoutsAsync(App app)
    {
        int[] incomingLayoutIds = app.Layouts
            .Where(layout => layout.Id > 0)
            .Select(layout => layout.Id)
            .ToArray();

        Layout[] layoutsToDelete = layoutOrchestrationService.GetAll(ignoreFilters: true)
            .Where(layout => layout.AppId == app.Id && !((ReadOnlySpan<int>)incomingLayoutIds).Contains(layout.Id))
            .ToArray();

        if (layoutsToDelete.Length > 0)
            await layoutOrchestrationService.DeleteAllAsync(layoutsToDelete);
    }

    private async ValueTask AddOrUpdateComponentsAsync(App app)
    {
        HashSet<int> existingComponentIds = componentOrchestrationService.GetAll(ignoreFilters: true)
            .Where(component => component.AppId == app.Id)
            .Select(component => component.Id)
            .ToHashSet();

        foreach (Component component in app.Components)
            if (existingComponentIds.Contains(component.Id))
                await componentOrchestrationService.UpdateAsync(component);
            else
                await componentOrchestrationService.AddAsync(component);
    }

    private static void ValidateApp(App app, string parameterName) =>
        ThrowIf(app == null, parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
            throw new ValidationException(message);
    }
}
