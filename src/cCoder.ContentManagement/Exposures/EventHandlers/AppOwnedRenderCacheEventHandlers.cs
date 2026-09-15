// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Brokers.Events;
using cCoder.ContentManagement.Services.Aggregations;

namespace cCoder.ContentManagement.Exposures.EventHandlers;

internal sealed class AppOwnedRenderCacheEventHandlers(IEventRegistrationBroker eventHub)
    : IContentManagementEventHandlers
{
    public void ListenToAllEvents() { }

    public void ListenToWebCacheEvents()
    {
        eventHub.ListenToEvent(
            name: "app_update",
            handler: (IPageRenderCacheAggregationService service, App app) =>
                service.DeleteAppAsync(appId: app.Id, fromEvent: true));

        eventHub.ListenToEvent(
            name: "app_delete",
            handler: (IPageRenderCacheAggregationService service, App app) =>
                service.DeleteAppAsync(appId: app.Id, fromEvent: true));

        eventHub.ListenToEvent<AppCulture, IPageRenderCacheAggregationService>(
            name: "app_culture_add",
            handler: static (service, appCulture) => service.DeleteAppAsync(appId: appCulture.AppId, fromEvent: true));

        eventHub.ListenToEvent<AppCulture, IPageRenderCacheAggregationService>(
            name: "app_culture_delete",
            handler: static (service, appCulture) => service.DeleteAppAsync(appId: appCulture.AppId, fromEvent: true));

        eventHub.ListenToEvent<Layout, IPageRenderCacheAggregationService>(
            name: "layout_add",
            handler: static (service, layout) => service.DeleteAppAsync(appId: layout.AppId, fromEvent: true));

        eventHub.ListenToEvent<Layout, IPageRenderCacheAggregationService>(
            name: "layout_update",
            handler: static (service, layout) => service.DeleteAppAsync(appId: layout.AppId, fromEvent: true));

        eventHub.ListenToEvent<Layout, IPageRenderCacheAggregationService>(
            name: "layout_delete",
            handler: static (service, layout) => service.DeleteAppAsync(appId: layout.AppId, fromEvent: true));

        eventHub.ListenToEvent<Template, IPageRenderCacheAggregationService>(
            name: "template_add",
            handler: static (service, template) => service.DeleteAppAsync(appId: template.AppId, fromEvent: true));

        eventHub.ListenToEvent<Template, IPageRenderCacheAggregationService>(
            name: "template_update",
            handler: static (service, template) => service.DeleteAppAsync(appId: template.AppId, fromEvent: true));

        eventHub.ListenToEvent<Template, IPageRenderCacheAggregationService>(
            name: "template_delete",
            handler: static (service, template) => service.DeleteAppAsync(appId: template.AppId, fromEvent: true));

        eventHub.ListenToEvent<Component, IPageRenderCacheAggregationService>(
            name: "component_add",
            handler: static (service, component) => service.DeleteAppAsync(appId: component.AppId, fromEvent: true));

        eventHub.ListenToEvent<Component, IPageRenderCacheAggregationService>(
            name: "component_update",
            handler: static (service, component) => service.DeleteAppAsync(appId: component.AppId, fromEvent: true));

        eventHub.ListenToEvent<Component, IPageRenderCacheAggregationService>(
            name: "component_delete",
            handler: static (service, component) => service.DeleteAppAsync(appId: component.AppId, fromEvent: true));

        eventHub.ListenToEvent<Resource, IPageRenderCacheAggregationService>(
            name: "resource_add",
            handler: static (service, resource) => service.DeleteAppAsync(appId: resource.AppId, fromEvent: true));

        eventHub.ListenToEvent<Resource, IPageRenderCacheAggregationService>(
            name: "resource_update",
            handler: static (service, resource) => service.DeleteAppAsync(appId: resource.AppId, fromEvent: true));

        eventHub.ListenToEvent<Resource, IPageRenderCacheAggregationService>(
            name: "resource_delete",
            handler: static (service, resource) => service.DeleteAppAsync(appId: resource.AppId, fromEvent: true));

        eventHub.ListenToEvent<Script, IPageRenderCacheAggregationService>(
            name: "script_add",
            handler: static (service, script) => service.DeleteAppAsync(appId: script.AppId, fromEvent: true));

        eventHub.ListenToEvent<Script, IPageRenderCacheAggregationService>(
            name: "script_update",
            handler: static (service, script) => service.DeleteAppAsync(appId: script.AppId, fromEvent: true));

        eventHub.ListenToEvent<Script, IPageRenderCacheAggregationService>(
            name: "script_delete",
            handler: static (service, script) => service.DeleteAppAsync(appId: script.AppId, fromEvent: true));
    }

    public void ListenToHostedEvents() { }

    public void ListenToFinalAppDeleteEvent() { }
}