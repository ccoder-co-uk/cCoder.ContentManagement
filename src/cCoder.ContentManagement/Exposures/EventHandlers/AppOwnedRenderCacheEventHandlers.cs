// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Brokers.Events;

namespace cCoder.ContentManagement.Exposures.EventHandlers;

internal sealed class AppOwnedRenderCacheEventHandlers(IEventRegistrationBroker eventHub)
    : IContentManagementEventHandlers
{
    public void ListenToAllEvents() { }

    public void ListenToWebCacheEvents()
    {
        eventHub.ListenToEvent(
            name: "app_update",
            handler: (IPageRenderCacheEventHandlers service, App app) =>
                service.InvalidateAppAsync(app: app));

        eventHub.ListenToEvent(
            name: "app_delete",
            handler: (IPageRenderCacheEventHandlers service, App app) =>
                service.DeleteAppAsync(deletedApp: app));

        eventHub.ListenToEvent<AppCulture, IPageRenderCacheEventHandlers>(
            name: "app_culture_add",
            handler: static (service, appCulture) => service.InvalidateAppAsync(appCulture: appCulture));

        eventHub.ListenToEvent<AppCulture, IPageRenderCacheEventHandlers>(
            name: "app_culture_delete",
            handler: static (service, appCulture) => service.InvalidateAppAsync(appCulture: appCulture));

        eventHub.ListenToEvent<Layout, IPageRenderCacheEventHandlers>(
            name: "layout_add",
            handler: static (service, layout) => service.InvalidateAppAsync(layout: layout));

        eventHub.ListenToEvent<Layout, IPageRenderCacheEventHandlers>(
            name: "layout_update",
            handler: static (service, layout) => service.InvalidateAppAsync(layout: layout));

        eventHub.ListenToEvent<Layout, IPageRenderCacheEventHandlers>(
            name: "layout_delete",
            handler: static (service, layout) => service.InvalidateAppAsync(layout: layout));

        eventHub.ListenToEvent<Template, IPageRenderCacheEventHandlers>(
            name: "template_add",
            handler: static (service, template) => service.InvalidateAppAsync(template: template));

        eventHub.ListenToEvent<Template, IPageRenderCacheEventHandlers>(
            name: "template_update",
            handler: static (service, template) => service.InvalidateAppAsync(template: template));

        eventHub.ListenToEvent<Template, IPageRenderCacheEventHandlers>(
            name: "template_delete",
            handler: static (service, template) => service.InvalidateAppAsync(template: template));

        eventHub.ListenToEvent<Component, IPageRenderCacheEventHandlers>(
            name: "component_add",
            handler: static (service, component) => service.InvalidateAppAsync(component: component));

        eventHub.ListenToEvent<Component, IPageRenderCacheEventHandlers>(
            name: "component_update",
            handler: static (service, component) => service.InvalidateAppAsync(component: component));

        eventHub.ListenToEvent<Component, IPageRenderCacheEventHandlers>(
            name: "component_delete",
            handler: static (service, component) => service.InvalidateAppAsync(component: component));

        eventHub.ListenToEvent<Resource, IPageRenderCacheEventHandlers>(
            name: "resource_add",
            handler: static (service, resource) => service.InvalidateAppAsync(resource: resource));

        eventHub.ListenToEvent<Resource, IPageRenderCacheEventHandlers>(
            name: "resource_update",
            handler: static (service, resource) => service.InvalidateAppAsync(resource: resource));

        eventHub.ListenToEvent<Resource, IPageRenderCacheEventHandlers>(
            name: "resource_delete",
            handler: static (service, resource) => service.InvalidateAppAsync(resource: resource));

        eventHub.ListenToEvent<Script, IPageRenderCacheEventHandlers>(
            name: "script_add",
            handler: static (service, script) => service.InvalidateAppAsync(script: script));

        eventHub.ListenToEvent<Script, IPageRenderCacheEventHandlers>(
            name: "script_update",
            handler: static (service, script) => service.InvalidateAppAsync(script: script));

        eventHub.ListenToEvent<Script, IPageRenderCacheEventHandlers>(
            name: "script_delete",
            handler: static (service, script) => service.InvalidateAppAsync(script: script));
    }

    public void ListenToHostedEvents() { }

    public void ListenToFinalAppDeleteEvent() { }
}