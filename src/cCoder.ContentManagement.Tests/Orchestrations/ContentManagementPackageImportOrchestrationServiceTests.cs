// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Foundations.Serialization;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Orchestrations;

public sealed partial class ContentManagementPackageImportOrchestrationServiceTests
{
    private const string CurrentUserId = "test-user";

    [Fact]
    public async Task ImportPackageAsync_WhenAppComponentItemProvided_RaisesTypedBatchEventAsync()
    {
        // Given
        const int appId = 17;

        Mock<IPackageImportEventProcessingService> eventService =
            new(behavior: MockBehavior.Strict);

        eventService.Setup(expression: service => service.RaiseImportAsync(
                eventName: "component_import",
                import: It.Is<PackageItemImportEvent<Component>>(match: match =>
                    match.AppId == appId
                    && match.Items.Length == 1
                    && match.Items[0].Name == "Navigation"),
                userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        ContentManagementPackageImportOrchestrationService service =
            CreateService(eventService: eventService.Object);

        Package package = new()
        {
            Items =
            [
                new PackageItem
                {
                    Type = "contentmanagement/component",
                    Data = """
                           {
                             "Name": "Navigation",
                             "Content": "<nav></nav>",
                             "Script": "",
                             "CreatedOn": "",
                             "LastUpdated": ""
                           }
                           """
                }
            ]
        };

        // When
        await service.ImportAppPackageAsync(appId: appId, package: package);

        // Then
        eventService.VerifyAll();
    }

    [Fact]
    public async Task ImportPackageAsync_WhenCommonCacheItemProvided_RaisesCommonObjectBatchEventAsync()
    {
        // Given
        Mock<IPackageImportEventProcessingService> eventService =
            new(behavior: MockBehavior.Strict);

        eventService.Setup(expression: service => service.RaiseImportAsync(
                eventName: "common_objects_import",
                import: It.Is<PackageItemImportEvent<CommonObject>>(match: match =>
                    match.AppId == null
                    && match.Items.Length == 1
                    && match.Items[0].Name == "Navigation"
                    && match.Items[0].Key == "Core"
                    && match.Items[0].Type == "Core/Component"),
                userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        ContentManagementPackageImportOrchestrationService service =
            CreateService(eventService: eventService.Object);

        Package package = new()
        {
            Category = "Baseline",
            Items =
            [
                new PackageItem
                {
                    Type = "Core/Component",
                    Data = """
                           {
                             "Name": "Navigation",
                             "ResourceKey": "Core",
                             "Content": "<nav></nav>"
                           }
                           """
                }
            ]
        };

        // When
        await service.ImportCommonCachePackageAsync(package: package);

        // Then
        eventService.VerifyAll();
    }

    private static ContentManagementPackageImportOrchestrationService CreateService(
        IPackageImportEventProcessingService eventService)
    {
        JsonBroker jsonBroker = new();
        Mock<IAuthorizationProcessingService> authorizationService = new();
        authorizationService
            .Setup(expression: service => service.GetCurrentUserId())
            .Returns(value: CurrentUserId);

        return new ContentManagementPackageImportOrchestrationService(
            jsonProcessingService: new JsonProcessingService(
                jsonService: new JsonService(jsonBroker: jsonBroker)),
            packageImportEventProcessingService: eventService,
            authorizationProcessingService: authorizationService.Object);
    }
}