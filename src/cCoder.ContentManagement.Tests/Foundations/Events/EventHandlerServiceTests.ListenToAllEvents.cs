// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Aggregations;
using cCoder.ContentManagement.Services.Coordinations;
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

        eventHubBrokerMock
            .Setup(expression: x => x.ListenToEvent<(int appId, Package package), IContentManagementMigrationAggregationService>(
eventName: "package_import",
handler: It.IsAny<Func<IContentManagementMigrationAggregationService, (int appId, Package package), ValueTask>>()));

        // When
        service.ListenToAllEvents();

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
    }

}