// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Aggregations;
using cCoder.ContentManagement.Services.Coordinations;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Exposures.EventHandlers;
using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Foundations.Events;

public partial class EventHandlerServiceTests
{
    [Fact]
    public void ShouldRegisterPassThroughAppPageAndPackageHandlers()
    {
        // Given
        SetupAppCoordinationEventRegistrations(eventName: "app_add");
        SetupAppCoordinationEventRegistrations(eventName: "app_update");
        SetupAppCoordinationEventRegistrations(eventName: "app_delete");
        SetupPageRenderCacheEventRegistrations();

        eventHubBrokerMock
            .Setup(expression: x => x.ListenToEvent<App, IAppOrchestrationService>(
eventName: "app_delete",
handler: It.IsAny<Func<IAppOrchestrationService, App, ValueTask>>()));

        eventHubBrokerMock
            .Setup(expression: x => x.ListenToEvent<Page, IPageCoordinationService>(
eventName: "page_add",
handler: It.IsAny<Func<IPageCoordinationService, Page, ValueTask>>()));

        eventHubBrokerMock
            .Setup(expression: x => x.ListenToEvent<Page, IPageCoordinationService>(
eventName: "page_update",
handler: It.IsAny<Func<IPageCoordinationService, Page, ValueTask>>()));

        eventHubBrokerMock
            .Setup(expression: x => x.ListenToEvent<Page, IPageCoordinationService>(
eventName: "page_delete",
handler: It.IsAny<Func<IPageCoordinationService, Page, ValueTask>>()));

        SetupPageStructureEventRegistration(eventName: "page_add");
        SetupPageStructureEventRegistration(eventName: "page_update");
        SetupPageStructureEventRegistration(eventName: "page_delete");

        eventHubBrokerMock
            .Setup(expression: x => x.ListenToEvent<(int appId, Package package), IContentManagementMigrationAggregationService>(
eventName: "package_import",
handler: It.IsAny<Func<IContentManagementMigrationAggregationService, (int appId, Package package), ValueTask>>()));

        eventHubBrokerMock
            .Setup(expression: x => x.ListenToEvent<(int appId, Package package), IPageRenderCacheEventHandlers>(
eventName: "package_import_complete",
handler: It.IsAny<Func<IPageRenderCacheEventHandlers, (int appId, Package package), ValueTask>>()));

        // When
        service.ListenToAllEvents();
        service.ListenToFinalAppDeleteEvent();

        // Then
        eventHubBrokerMock.VerifyAll();
    }

    private void SetupAppCoordinationEventRegistrations(string eventName)
    {
        eventHubBrokerMock
            .Setup(expression: x => x.ListenToEvent<App, IAppSupportingResourcesCoordinationService>(
eventName: eventName,
handler: It.IsAny<Func<IAppSupportingResourcesCoordinationService, App, ValueTask>>()));

        eventHubBrokerMock
            .Setup(expression: x => x.ListenToEvent<App, IAppRenderableCoordinationService>(
eventName: eventName,
handler: It.IsAny<Func<IAppRenderableCoordinationService, App, ValueTask>>()));

        eventHubBrokerMock
            .Setup(expression: x => x.ListenToEvent<App, IAppPageComponentCoordinationService>(
eventName: eventName,
handler: It.IsAny<Func<IAppPageComponentCoordinationService, App, ValueTask>>()));
    }

    private void SetupPageStructureEventRegistration(string eventName) =>
        eventHubBrokerMock
            .Setup(expression: broker => broker.ListenToEvent<Page, IPageStructureCoordinationService>(
eventName: eventName,
handler: It.IsAny<Func<IPageStructureCoordinationService, Page, ValueTask>>()));

    private void SetupPageRenderCacheEventRegistrations()
    {
        eventHubBrokerMock
            .Setup(expression: broker => broker.ListenToEvent<App, IPageRenderCacheEventHandlers>(
                eventName: It.IsAny<string>(),
                handler: It.IsAny<Func<IPageRenderCacheEventHandlers, App, ValueTask>>()));

        eventHubBrokerMock
            .Setup(expression: broker => broker.ListenToEvent<Page, IPageRenderCacheEventHandlers>(
                eventName: It.IsAny<string>(),
                handler: It.IsAny<Func<IPageRenderCacheEventHandlers, Page, ValueTask>>()));

        eventHubBrokerMock
            .Setup(expression: broker => broker.ListenToEvent<AppCulture, IPageRenderCacheEventHandlers>(
                eventName: It.IsAny<string>(),
                handler: It.IsAny<Func<IPageRenderCacheEventHandlers, AppCulture, ValueTask>>()));

        eventHubBrokerMock
            .Setup(expression: broker => broker.ListenToEvent<Layout, IPageRenderCacheEventHandlers>(
                eventName: It.IsAny<string>(),
                handler: It.IsAny<Func<IPageRenderCacheEventHandlers, Layout, ValueTask>>()));

        eventHubBrokerMock
            .Setup(expression: broker => broker.ListenToEvent<Template, IPageRenderCacheEventHandlers>(
                eventName: It.IsAny<string>(),
                handler: It.IsAny<Func<IPageRenderCacheEventHandlers, Template, ValueTask>>()));

        eventHubBrokerMock
            .Setup(expression: broker => broker.ListenToEvent<Component, IPageRenderCacheEventHandlers>(
                eventName: It.IsAny<string>(),
                handler: It.IsAny<Func<IPageRenderCacheEventHandlers, Component, ValueTask>>()));

        eventHubBrokerMock
            .Setup(expression: broker => broker.ListenToEvent<Resource, IPageRenderCacheEventHandlers>(
                eventName: It.IsAny<string>(),
                handler: It.IsAny<Func<IPageRenderCacheEventHandlers, Resource, ValueTask>>()));

        eventHubBrokerMock
            .Setup(expression: broker => broker.ListenToEvent<Script, IPageRenderCacheEventHandlers>(
                eventName: It.IsAny<string>(),
                handler: It.IsAny<Func<IPageRenderCacheEventHandlers, Script, ValueTask>>()));

        eventHubBrokerMock
            .Setup(expression: broker => broker.ListenToEvent<Content, IPageRenderCacheEventHandlers>(
                eventName: It.IsAny<string>(),
                handler: It.IsAny<Func<IPageRenderCacheEventHandlers, Content, ValueTask>>()));

        eventHubBrokerMock
            .Setup(expression: broker => broker.ListenToEvent<PageInfo, IPageRenderCacheEventHandlers>(
                eventName: It.IsAny<string>(),
                handler: It.IsAny<Func<IPageRenderCacheEventHandlers, PageInfo, ValueTask>>()));

        eventHubBrokerMock
            .Setup(expression: broker => broker.ListenToEvent<CommonObject, IPageRenderCacheEventHandlers>(
                eventName: It.IsAny<string>(),
                handler: It.IsAny<Func<IPageRenderCacheEventHandlers, CommonObject, ValueTask>>()));

        eventHubBrokerMock
            .Setup(expression: broker => broker.ListenToEvent<PageRenderCacheMiss, IPageRenderCacheMissEventHandler>(
                eventName: "page_render_cache_miss",
                handler: It.IsAny<Func<IPageRenderCacheMissEventHandler, PageRenderCacheMiss, ValueTask>>()));
    }

}