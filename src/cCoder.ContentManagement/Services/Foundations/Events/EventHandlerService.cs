using cCoder.ContentManagement.Brokers.Events;
using cCoder.ContentManagement.Services.Aggregations;
using cCoder.ContentManagement.Services.Coordinations;
using cCoder.Data.Models.Packaging;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Events;

internal partial class EventHandlerService(IEventHubBroker eventHubBroker) : IEventHandlerService
{
    public void ListenToAllEvents()
    {
        ValidateEventHubBroker(eventHubBroker, "eventHubBroker");
        ListenToAppEvents();
        ListenToPageEvents();
        ListenToPackageEvents();
    }

    private void ListenToAppEvents()
    {
        ListenToAppAddEvents();
        ListenToAppUpdateEvents();
        ListenToAppDeleteEvents();
    }

    private void ListenToPageEvents()
    {
        ListenToPageAddEvents();
        ListenToPageUpdateEvents();
        ListenToPageDeleteEvents();
    }

    private void ListenToPackageEvents()
    {
        ListenToPackageImportEvents();
    }

    private void ListenToAppAddEvents()
    {
        eventHubBroker.ListenToEvent("app_add", async (IAppCultureOrchestrationService service, App app) =>
        {
            AppCulture[] cultures = (app.Cultures ?? []).ToArray();
            Array.ForEach(cultures, culture => culture.AppId = app.Id);
            await service.AddOrUpdate(cultures);
        });
        eventHubBroker.ListenToEvent("app_add", async (IComponentOrchestrationService service, App app) =>
        {
            Component[] components = (app.Components ?? []).ToArray();
            Array.ForEach(components, component => component.AppId = app.Id);
            await service.AddOrUpdate(components);
        });
        eventHubBroker.ListenToEvent("app_add", async (ILayoutOrchestrationService service, App app) =>
        {
            Layout[] layouts = (app.Layouts ?? []).ToArray();
            Array.ForEach(layouts, layout => layout.AppId = app.Id);
            await service.AddOrUpdate(layouts);
        });
        eventHubBroker.ListenToEvent("app_add", async (IPageOrchestrationService service, App app) =>
        {
            Page[] pages = (app.Pages ?? []).ToArray();
            Array.ForEach(pages, page => page.AppId = app.Id);
            await service.AddOrUpdate(pages);
        });
        eventHubBroker.ListenToEvent("app_add", async (IResourceOrchestrationService service, App app) =>
        {
            Resource[] resources = (app.Resources ?? []).ToArray();
            Array.ForEach(resources, resource => resource.AppId = app.Id);
            await service.AddOrUpdate(resources);
        });
        eventHubBroker.ListenToEvent("app_add", async (IScriptOrchestrationService service, App app) =>
        {
            Script[] scripts = (app.Scripts ?? []).ToArray();
            Array.ForEach(scripts, script => script.AppId = app.Id);
            await service.AddOrUpdate(scripts);
        });
        eventHubBroker.ListenToEvent("app_add", async (ITemplateOrchestrationService service, App app) =>
        {
            Template[] templates = (app.Templates ?? []).ToArray();
            Array.ForEach(templates, template => template.AppId = app.Id);
            await service.AddOrUpdate(templates);
        });
    }

    private void ListenToAppUpdateEvents()
    {
        eventHubBroker.ListenToEvent("app_update", async (IAppCultureOrchestrationService service, App app) =>
        {
            if (app.Cultures == null)
            {
                return;
            }

            AppCulture[] cultures = app.Cultures.ToArray();
            Array.ForEach(cultures, culture => culture.AppId = app.Id);
            await service.AddOrUpdate(cultures);

            string[] incomingCultureIds = cultures
                .Where(culture => culture.CultureId != string.Empty)
                .Select(culture => culture.CultureId)
                .Distinct(StringComparer.Ordinal)
                .ToArray();

            AppCulture[] culturesToDelete = service.GetAll(ignoreFilters: true)
                .Where(culture =>
                    culture.AppId == app.Id &&
                    culture.CultureId != string.Empty &&
                    !incomingCultureIds.Contains(culture.CultureId))
                .ToArray();

            if (culturesToDelete.Length > 0)
            {
                await service.DeleteAllAsync(culturesToDelete);
            }
        });
        eventHubBroker.ListenToEvent("app_update", async (IComponentOrchestrationService service, App app) =>
        {
            Component[] components = (app.Components ?? []).ToArray();
            Array.ForEach(components, component => component.AppId = app.Id);
            await service.AddOrUpdate(components);
        });
        eventHubBroker.ListenToEvent("app_update", async (ILayoutOrchestrationService service, App app) =>
        {
            Layout[] layouts = (app.Layouts ?? []).ToArray();
            Array.ForEach(layouts, layout => layout.AppId = app.Id);
            await service.AddOrUpdate(layouts);
        });
        eventHubBroker.ListenToEvent("app_update", async (IPageOrchestrationService service, App app) =>
        {
            Page[] pages = (app.Pages ?? []).ToArray();
            Array.ForEach(pages, page => page.AppId = app.Id);
            await service.AddOrUpdate(pages);
        });
        eventHubBroker.ListenToEvent("app_update", async (IResourceOrchestrationService service, App app) =>
        {
            Resource[] resources = (app.Resources ?? []).ToArray();
            Array.ForEach(resources, resource => resource.AppId = app.Id);
            await service.AddOrUpdate(resources);
        });
        eventHubBroker.ListenToEvent("app_update", async (IScriptOrchestrationService service, App app) =>
        {
            Script[] scripts = (app.Scripts ?? []).ToArray();
            Array.ForEach(scripts, script => script.AppId = app.Id);
            await service.AddOrUpdate(scripts);
        });
        eventHubBroker.ListenToEvent("app_update", async (ITemplateOrchestrationService service, App app) =>
        {
            Template[] templates = (app.Templates ?? []).ToArray();
            Array.ForEach(templates, template => template.AppId = app.Id);
            await service.AddOrUpdate(templates);
        });
    }

    private void ListenToAppDeleteEvents()
    {
        eventHubBroker.ListenToEvent("app_delete", (IAppCultureOrchestrationService service, App app) => service.DeleteByAppIdAsync(app.Id));
        eventHubBroker.ListenToEvent("app_delete", (IComponentOrchestrationService service, App app) => service.DeleteByAppIdAsync(app.Id));
        eventHubBroker.ListenToEvent("app_delete", (ILayoutOrchestrationService service, App app) => service.DeleteByAppIdAsync(app.Id));
        eventHubBroker.ListenToEvent("app_delete", (IPageOrchestrationService service, App app) => service.DeleteByAppIdAsync(app.Id));
        eventHubBroker.ListenToEvent("app_delete", (IResourceOrchestrationService service, App app) => service.DeleteByAppIdAsync(app.Id));
        eventHubBroker.ListenToEvent("app_delete", (IScriptOrchestrationService service, App app) => service.DeleteByAppIdAsync(app.Id));
        eventHubBroker.ListenToEvent("app_delete", (ITemplateOrchestrationService service, App app) => service.DeleteByAppIdAsync(app.Id));
    }

    private void ListenToPageAddEvents()
    {
        eventHubBroker.ListenToEvent("page_add", (IPageCoordinationService service, Page page) => service.HandlePageAddAsync(page));
    }

    private void ListenToPageUpdateEvents()
    {
        eventHubBroker.ListenToEvent("page_update", (IPageCoordinationService service, Page page) => service.HandlePageUpdateAsync(page));
    }

    private void ListenToPageDeleteEvents()
    {
        eventHubBroker.ListenToEvent("page_delete", (IPageCoordinationService service, Page page) => service.HandlePageDeleteAsync(page));
    }

    private void ListenToPackageImportEvents()
    {
        eventHubBroker.ListenToEvent("package_import", (IContentManagementMigrationAggregationService service, (int appId, Package package) args) => service.ImportPackageAsync(args.appId, args.package));
    }
}
