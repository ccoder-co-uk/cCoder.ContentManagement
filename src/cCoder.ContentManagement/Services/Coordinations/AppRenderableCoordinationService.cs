using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.ContentManagement.Services.Orchestrations;
using App = cCoder.Data.Models.CMS.App;
using Component = cCoder.Data.Models.CMS.Component;
using Layout = cCoder.Data.Models.CMS.Layout;
using Page = cCoder.Data.Models.CMS.Page;
using Template = cCoder.Data.Models.CMS.Template;

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
        await templateOrchestrationService.AddOrUpdate(app.Templates ?? Array.Empty<Template>());
        await layoutOrchestrationService.AddOrUpdate(app.Layouts ?? Array.Empty<Layout>());
        await AddOrUpdateComponentsAsync(app);
        await pageOrchestrationService.AddOrUpdate(app.Pages ?? Array.Empty<Page>());
    }

    public async ValueTask HandleAppUpdateAsync(App app)
    {
        ValidateApp(app, "app");
        StampChildrenWithApp(app);
        await DeleteMissingPagesAsync(app);
        await DeleteMissingComponentsAsync(app);
        await DeleteMissingTemplatesAsync(app);
        await DeleteMissingLayoutsAsync(app);
        await templateOrchestrationService.AddOrUpdate(app.Templates ?? Array.Empty<Template>());
        await layoutOrchestrationService.AddOrUpdate(app.Layouts ?? Array.Empty<Layout>());
        await AddOrUpdateComponentsAsync(app);
        await pageOrchestrationService.AddOrUpdate(app.Pages ?? Array.Empty<Page>());
    }

    public async ValueTask HandleAppDeleteAsync(App app)
    {
        ValidateApp(app, "app");
        IEnumerable<Page> pagesToDelete = (from page in pageOrchestrationService.GetAll(ignoreFilters: true)
                                           where page.AppId == app.Id
                                           select page).ToArray();
        cCoder.Data.Models.CMS.Component[] componentsToDelete = componentOrchestrationService.GetAll(ignoreFilters: true)
            .Where(component => component.AppId == app.Id)
            .ToArray();
        IEnumerable<Template> templatesToDelete = (from template in templateOrchestrationService.GetAll(ignoreFilters: true)
                                                   where template.AppId == app.Id
                                                   select template).ToArray();
        IEnumerable<Layout> layoutsToDelete = (from layout in layoutOrchestrationService.GetAll(ignoreFilters: true)
                                               where layout.AppId == app.Id
                                               select layout).ToArray();
        await pageOrchestrationService.DeleteAllAsync(pagesToDelete);
        if (componentsToDelete.Length > 0)
        {
            await componentOrchestrationService.DeleteAllAsync(componentsToDelete);
        }
        await templateOrchestrationService.DeleteAllAsync(templatesToDelete);
        await layoutOrchestrationService.DeleteAllAsync(layoutsToDelete);
    }

    private static void StampChildrenWithApp(App app)
    {
        foreach (Page item in app.Pages ?? new List<Page>())
        {
            item.AppId = app.Id;
        }
        foreach (Component item2 in app.Components ?? new List<Component>())
        {
            item2.AppId = app.Id;
        }
        foreach (Template item3 in app.Templates ?? new List<Template>())
        {
            item3.AppId = app.Id;
        }
        foreach (Layout item4 in app.Layouts ?? new List<Layout>())
        {
            item4.AppId = app.Id;
        }
    }

    private async ValueTask DeleteMissingPagesAsync(App app)
    {
        int[] incomingPageIds = (app.Pages ?? Array.Empty<Page>())
            .Where(page => page.Id > 0)
            .Select(page => page.Id)
            .ToArray();

        Page[] pagesToDelete = pageOrchestrationService.GetAll(ignoreFilters: true)
            .Where(page => page.AppId == app.Id && !((ReadOnlySpan<int>)incomingPageIds).Contains(page.Id))
            .ToArray();

        if (pagesToDelete.Length > 0)
        {
            await pageOrchestrationService.DeleteAllAsync(pagesToDelete);
        }
    }

    private async ValueTask DeleteMissingComponentsAsync(App app)
    {
        int[] incomingComponentIds = (app.Components ?? Array.Empty<Component>())
            .Where(component => component.Id > 0)
            .Select(component => component.Id)
            .ToArray();

        cCoder.Data.Models.CMS.Component[] componentsToDelete = componentOrchestrationService.GetAll(ignoreFilters: true)
            .Where(component => component.AppId == app.Id && !((ReadOnlySpan<int>)incomingComponentIds).Contains(component.Id))
            .ToArray();

        if (componentsToDelete.Length > 0)
        {
            await componentOrchestrationService.DeleteAllAsync(componentsToDelete);
        }
    }

    private async ValueTask DeleteMissingTemplatesAsync(App app)
    {
        int[] incomingTemplateIds = (app.Templates ?? Array.Empty<Template>())
            .Where(template => template.Id > 0)
            .Select(template => template.Id)
            .ToArray();

        Template[] templatesToDelete = templateOrchestrationService.GetAll(ignoreFilters: true)
            .Where(template => template.AppId == app.Id && !((ReadOnlySpan<int>)incomingTemplateIds).Contains(template.Id))
            .ToArray();

        if (templatesToDelete.Length > 0)
        {
            await templateOrchestrationService.DeleteAllAsync(templatesToDelete);
        }
    }

    private async ValueTask DeleteMissingLayoutsAsync(App app)
    {
        int[] incomingLayoutIds = (app.Layouts ?? Array.Empty<Layout>())
            .Where(layout => layout.Id > 0)
            .Select(layout => layout.Id)
            .ToArray();

        Layout[] layoutsToDelete = layoutOrchestrationService.GetAll(ignoreFilters: true)
            .Where(layout => layout.AppId == app.Id && !((ReadOnlySpan<int>)incomingLayoutIds).Contains(layout.Id))
            .ToArray();

        if (layoutsToDelete.Length > 0)
        {
            await layoutOrchestrationService.DeleteAllAsync(layoutsToDelete);
        }
    }

    private async ValueTask AddOrUpdateComponentsAsync(App app)
    {
        HashSet<int> existingComponentIds = componentOrchestrationService.GetAll(ignoreFilters: true)
            .Where(component => component.AppId == app.Id)
            .Select(component => component.Id)
            .ToHashSet();

        foreach (Component component in app.Components ?? Array.Empty<Component>())
        {
            if (existingComponentIds.Contains(component.Id))
            {
                await componentOrchestrationService.UpdateAsync(component);
            }
            else
            {
                await componentOrchestrationService.AddAsync(component);
            }
        }
    }

    private static App ValidateApp(App app, string parameterName)
    {
        if (app == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
        return app;
    }
}
