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
        SetupAppCoordinationEventRegistrations("app_add");
        SetupAppCoordinationEventRegistrations("app_update");
        SetupAppCoordinationEventRegistrations("app_delete");
        eventHubBrokerMock
            .Setup(x => x.ListenToEvent<Page, IPageCoordinationService>(
                "page_add",
                It.IsAny<Func<IPageCoordinationService, Page, ValueTask>>()));
        eventHubBrokerMock
            .Setup(x => x.ListenToEvent<Page, IPageCoordinationService>(
                "page_update",
                It.IsAny<Func<IPageCoordinationService, Page, ValueTask>>()));
        eventHubBrokerMock
            .Setup(x => x.ListenToEvent<Page, IPageCoordinationService>(
                "page_delete",
                It.IsAny<Func<IPageCoordinationService, Page, ValueTask>>()));
        eventHubBrokerMock
            .Setup(x => x.ListenToEvent<(int appId, Package package), IContentManagementMigrationAggregationService>(
                "package_import",
                It.IsAny<Func<IContentManagementMigrationAggregationService, (int appId, Package package), ValueTask>>()));

        // When
        service.ListenToAllEvents();

        // Then
        eventHubBrokerMock.VerifyAll();
    }

    private void SetupAppCoordinationEventRegistrations(string eventName)
    {
        eventHubBrokerMock
            .Setup(x => x.ListenToEvent<App, IAppSupportingResourcesCoordinationService>(
                eventName,
                It.IsAny<Func<IAppSupportingResourcesCoordinationService, App, ValueTask>>()));
        eventHubBrokerMock
            .Setup(x => x.ListenToEvent<App, IAppRenderableCoordinationService>(
                eventName,
                It.IsAny<Func<IAppRenderableCoordinationService, App, ValueTask>>()));
    }

}
