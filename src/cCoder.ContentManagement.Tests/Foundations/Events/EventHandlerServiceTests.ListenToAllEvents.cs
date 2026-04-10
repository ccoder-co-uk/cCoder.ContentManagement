using cCoder.ContentManagement.Services.Aggregations;
using cCoder.ContentManagement.Services.Coordinations;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Foundations.Events;

public partial class EventHandlerServiceTests
{
    [Fact]
    public void ShouldRegisterDirectAppChildAndExistingPagePackageHandlers()
    {
        // Given
        SetupAppEventRegistrations("app_add");
        SetupAppEventRegistrations("app_update");
        SetupAppDeleteRegistrations();
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

    private void SetupAppEventRegistrations(string eventName)
    {
        eventHubBrokerMock
            .Setup(x => x.ListenToEvent<App, IAppCultureOrchestrationService>(
                eventName,
                It.IsAny<Func<IAppCultureOrchestrationService, App, ValueTask>>()));
        eventHubBrokerMock
            .Setup(x => x.ListenToEvent<App, IComponentOrchestrationService>(
                eventName,
                It.IsAny<Func<IComponentOrchestrationService, App, ValueTask>>()));
        eventHubBrokerMock
            .Setup(x => x.ListenToEvent<App, ILayoutOrchestrationService>(
                eventName,
                It.IsAny<Func<ILayoutOrchestrationService, App, ValueTask>>()));
        eventHubBrokerMock
            .Setup(x => x.ListenToEvent<App, IPageOrchestrationService>(
                eventName,
                It.IsAny<Func<IPageOrchestrationService, App, ValueTask>>()));
        eventHubBrokerMock
            .Setup(x => x.ListenToEvent<App, IResourceOrchestrationService>(
                eventName,
                It.IsAny<Func<IResourceOrchestrationService, App, ValueTask>>()));
        eventHubBrokerMock
            .Setup(x => x.ListenToEvent<App, IScriptOrchestrationService>(
                eventName,
                It.IsAny<Func<IScriptOrchestrationService, App, ValueTask>>()));
        eventHubBrokerMock
            .Setup(x => x.ListenToEvent<App, ITemplateOrchestrationService>(
                eventName,
                It.IsAny<Func<ITemplateOrchestrationService, App, ValueTask>>()));
    }

    private void SetupAppDeleteRegistrations() => SetupAppEventRegistrations("app_delete");
}
