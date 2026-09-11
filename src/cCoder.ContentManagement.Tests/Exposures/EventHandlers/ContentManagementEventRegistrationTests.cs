// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Exposures.EventHandlers;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Aggregations;
using cCoder.ContentManagement.Services.Coordinations;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.Packaging;
using cCoder.Eventing;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Exposures.EventHandlers;

public sealed partial class ContentManagementEventRegistrationTests
{
    public static TheoryData<Type, Type, string[]> RegistrationCases => new()
    {
        { typeof(AppSupportingResourcesEventHandlers), typeof(IAppSupportingResourcesCoordinationService), ["app_add", "app_update", "app_delete"] },
        { typeof(AppRenderableEventHandlers), typeof(IAppRenderableCoordinationService), ["app_add", "app_update", "app_delete"] },
        { typeof(AppPageComponentEventHandlers), typeof(IAppPageComponentCoordinationService), ["app_add", "app_update", "app_delete"] },
        { typeof(PageCoordinationEventHandlers), typeof(IPageCoordinationService), ["page_add", "page_update", "page_delete"] },
        { typeof(PageStructureEventHandlers), typeof(IPageStructureCoordinationService), ["page_add", "page_update", "page_delete"] },
    };

    [Fact]
    public void ContentManagement_WhenConfigured_ShouldRegisterEveryEventHandlerExposure()
    {
        // Given
        ServiceCollection services = new();

        // When
        services.AddContentManagementHostedServices();

        // Then
        services.Where(predicate: descriptor =>
                descriptor.ServiceType == typeof(IContentManagementEventHandlers))
            .Select(selector: descriptor => descriptor.ImplementationType)
            .Should()
            .BeEquivalentTo(expectation:
            [
                typeof(AppSupportingResourcesEventHandlers),
                typeof(AppRenderableEventHandlers),
                typeof(AppPageComponentEventHandlers),
                typeof(PageCoordinationEventHandlers),
                typeof(PageStructureEventHandlers),
                typeof(FinalAppDeleteEventHandlers),
                typeof(AppOwnedRenderCacheEventHandlers),
                typeof(PageOwnedRenderCacheEventHandlers),
                typeof(CommonObjectRenderCacheEventHandlers),
                typeof(PackageImportRenderCacheEventHandlers)
            ]);
    }

    [Fact]
    public void ContentManagement_WhenConfigured_ShouldRegisterAppManagerAggregation()
    {
        // Given
        ServiceCollection services = new();

        // When
        services.AddContentManagementHostedServices();

        // Then
        services.Should()
            .ContainSingle(predicate: descriptor =>
            descriptor.ServiceType == typeof(IAppManagerAggregationService) &&
            descriptor.ImplementationType == typeof(AppManagerAggregationService));
    }

    [Theory]
    [MemberData(nameof(RegistrationCases))]
    public void EntityEventHandlers_WhenListening_ShouldRegisterExpectedBusinessBoundary(
        Type eventHandlersType,
        Type expectedServiceType,
        string[] expectedEventNames)
    {
        // Given
        Mock<IEventHub> eventHubMock = new();

        IContentManagementEventHandlers eventHandlers =
            (IContentManagementEventHandlers)Activator.CreateInstance(
                type: eventHandlersType,
                args: eventHubMock.Object);

        // When
        eventHandlers.ListenToAllEvents();

        // Then
        eventHubMock.Invocations.Select(
            selector: invocation => invocation.Arguments[0])
            .Should()
            .Equal(expected: expectedEventNames);

        eventHubMock.Invocations.Should()
            .OnlyContain(
                predicate: invocation =>
                    invocation.Method.GetGenericArguments()[1] == expectedServiceType);
    }

    [Fact]
    public void FinalAppDeleteEventHandlers_WhenListening_ShouldRegisterFinalDeleteBoundary()
    {
        // Given
        Mock<IEventHub> eventHubMock = new();
        FinalAppDeleteEventHandlers eventHandlers = new(eventHub: eventHubMock.Object);

        // When
        eventHandlers.ListenToFinalAppDeleteEvent();

        // Then
        eventHubMock.Invocations.Should()
            .ContainSingle();

        eventHubMock.Invocations[0].Arguments[0].Should()
            .Be(expected: "app_delete");

        eventHubMock.Invocations[0].Method.GetGenericArguments()[1]
            .Should()
            .Be(expected: typeof(IAppOrchestrationService));
    }

    [Fact]
    public async Task PageRenderCacheEventHandlers_WhenPackageImportCompletes_ShouldSelectCorrectInvalidationAsync()
    {
        // Given
        const int appId = 23;
        Mock<IEventHub> eventHubMock = new();
        PackageImportRenderCacheEventHandlers eventHandlers = new(eventHub: eventHubMock.Object);
        Mock<IPageRenderCacheEventHandlers> cacheHandlersMock = new(MockBehavior.Strict);

        cacheHandlersMock.Setup(
            expression: service => service.InvalidatePackageAsync(appId: appId))
            .Returns(value: ValueTask.CompletedTask);

        cacheHandlersMock.Setup(
            expression: service => service.InvalidatePackageAsync(appId: null))
            .Returns(value: ValueTask.CompletedTask);

        eventHandlers.ListenToWebCacheEvents();

        Func<IPageRenderCacheEventHandlers, PackageImportEvent, ValueTask> packageHandler =
            (Func<IPageRenderCacheEventHandlers, PackageImportEvent, ValueTask>)eventHubMock
                .Invocations.Single(predicate: invocation =>
                    invocation.Arguments[0] as string == "package_import_complete")
                .Arguments[1];

        // When
        await packageHandler(
            arg1: cacheHandlersMock.Object,
            arg2: new PackageImportEvent { AppId = appId, Package = new Package() });

        await packageHandler(
            arg1: cacheHandlersMock.Object,
            arg2: new PackageImportEvent { AppId = null, Package = new Package() });

        // Then
        cacheHandlersMock.VerifyAll();

        eventHubMock.Invocations.Select(
            selector: invocation => invocation.Arguments[0])
            .Should()
            .NotContain(unexpected:
                ["uncached_page_render", "package_import", "app_package_import_complete"]);
    }
}