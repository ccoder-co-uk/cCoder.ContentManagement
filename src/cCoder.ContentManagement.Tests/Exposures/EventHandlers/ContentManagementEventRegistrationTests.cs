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

public sealed class ContentManagementEventRegistrationTests
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
        ServiceCollection services = new();

        services.AddContentManagementHostedServices();

        services.Where(descriptor =>
                descriptor.ServiceType == typeof(IContentManagementEventHandlers))
            .Select(descriptor => descriptor.ImplementationType)
            .Should().BeEquivalentTo(
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
        ServiceCollection services = new();

        services.AddContentManagementHostedServices();

        services.Should().ContainSingle(descriptor =>
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
        Mock<IEventHub> eventHubMock = new();
        IContentManagementEventHandlers eventHandlers =
            (IContentManagementEventHandlers)Activator.CreateInstance(
                type: eventHandlersType,
                args: eventHubMock.Object);

        eventHandlers.ListenToAllEvents();

        eventHubMock.Invocations.Select(invocation => invocation.Arguments[0])
            .Should().Equal(expectedEventNames);

        eventHubMock.Invocations.Should().OnlyContain(
            invocation => invocation.Method.GetGenericArguments()[1] == expectedServiceType);
    }

    [Fact]
    public void FinalAppDeleteEventHandlers_WhenListening_ShouldRegisterFinalDeleteBoundary()
    {
        Mock<IEventHub> eventHubMock = new();
        FinalAppDeleteEventHandlers eventHandlers = new(eventHub: eventHubMock.Object);

        eventHandlers.ListenToFinalAppDeleteEvent();

        eventHubMock.Invocations.Should().ContainSingle();
        eventHubMock.Invocations[0].Arguments[0].Should().Be("app_delete");
        eventHubMock.Invocations[0].Method.GetGenericArguments()[1]
            .Should().Be(typeof(IAppOrchestrationService));
    }

    [Fact]
    public async Task PageRenderCacheEventHandlers_WhenPackageImportCompletes_ShouldSelectCorrectInvalidationAsync()
    {
        const int appId = 23;
        Mock<IEventHub> eventHubMock = new();
        PackageImportRenderCacheEventHandlers eventHandlers = new(eventHub: eventHubMock.Object);
        Mock<IPageRenderCacheEventHandlers> cacheHandlersMock = new(MockBehavior.Strict);

        cacheHandlersMock.Setup(service => service.InvalidatePackageAsync(appId))
            .Returns(ValueTask.CompletedTask);
        cacheHandlersMock.Setup(service => service.InvalidatePackageAsync(null))
            .Returns(ValueTask.CompletedTask);

        eventHandlers.ListenToWebCacheEvents();

        Func<IPageRenderCacheEventHandlers, PackageImportEvent, ValueTask> packageHandler =
            (Func<IPageRenderCacheEventHandlers, PackageImportEvent, ValueTask>)eventHubMock
                .Invocations.Single(invocation =>
                    invocation.Arguments[0] as string == "package_import_complete")
                .Arguments[1];

        await packageHandler(
            cacheHandlersMock.Object,
            new PackageImportEvent { AppId = appId, Package = new Package() });

        await packageHandler(
            cacheHandlersMock.Object,
            new PackageImportEvent { AppId = null, Package = new Package() });

        cacheHandlersMock.VerifyAll();

        eventHubMock.Invocations.Select(invocation => invocation.Arguments[0])
            .Should().NotContain(["uncached_page_render", "package_import", "app_package_import_complete"]);
    }
}