// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Aggregations;
using cCoder.ContentManagement.Services.Coordinations;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Rendering.Services.Processings;
using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using cCoder.Eventing;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Exposures.EventHandlers;

public sealed partial class IEventHubExtensionsTests
{
    [Fact]
    public void ContentManagementEventStartup_WhenInspected_IsExposedFromIEventHubExtensions()
    {
        // Given
        Type contentManagementAssemblyMarker = typeof(IServiceCollectionExtensions);

        // When
        Type eventHubExtensionsType = contentManagementAssemblyMarker.Assembly
            .GetType(name: "cCoder.ContentManagement.IEventHubExtensions");

        // Then
        eventHubExtensionsType
            .Should()
            .NotBeNull();

        eventHubExtensionsType!
            .GetMethods()
            .Where(predicate: method => method.IsPublic && method.IsStatic)
            .Where(predicate: method => method.Name.StartsWith(
                value: "ListenToContentManagement",
                comparisonType: StringComparison.Ordinal))
            .Select(selector: method => method.GetParameters()[0].ParameterType)
            .Should()
            .OnlyContain(predicate: receiverType => receiverType == typeof(IEventHub));
    }

    [Fact]
    public void ContentManagementEventListening_WhenInspected_HasNoIntermediateHandlerOrBrokerTypes()
    {
        // Given
        Type contentManagementAssemblyMarker = typeof(IServiceCollectionExtensions);

        // When
        Type[] eventListeningInfrastructure = contentManagementAssemblyMarker.Assembly
            .GetTypes()
            .Where(predicate: type =>
                type.Namespace?.StartsWith(
                    value: "cCoder.ContentManagement.Exposures.EventHandlers",
                    comparisonType: StringComparison.Ordinal) == true ||
                type.Name is "EventRegistrationBroker" or "IEventRegistrationBroker")
            .ToArray();

        // Then
        eventListeningInfrastructure
            .Should()
            .BeEmpty(
                because: "IEventHubExtensions should wire registrations directly on the established event hub boundary");
    }

    [Fact]
    public void ContentManagementWebEvents_WhenStarted_RegisterExpectedBusinessBoundaries()
    {
        // Given
        Mock<IEventHub> eventHubMock = new();

        (string EventName, Type ServiceType)[] expectedRegistrations =
        [
            ("app_add", typeof(IAppSupportingResourcesCoordinationService)),
            ("app_update", typeof(IAppSupportingResourcesCoordinationService)),
            ("app_delete", typeof(IAppSupportingResourcesCoordinationService)),
            ("app_add", typeof(IAppRenderableCoordinationService)),
            ("app_update", typeof(IAppRenderableCoordinationService)),
            ("app_delete", typeof(IAppRenderableCoordinationService)),
            ("app_add", typeof(IAppPageComponentCoordinationService)),
            ("app_update", typeof(IAppPageComponentCoordinationService)),
            ("app_delete", typeof(IAppPageComponentCoordinationService)),
            ("page_add", typeof(IPageCoordinationService)),
            ("page_update", typeof(IPageCoordinationService)),
            ("page_delete", typeof(IPageCoordinationService)),
            ("page_add", typeof(IPageStructureCoordinationService)),
            ("page_update", typeof(IPageStructureCoordinationService)),
            ("page_delete", typeof(IPageStructureCoordinationService)),
            ("component_import", typeof(IComponentOrchestrationService)),
            ("layout_import", typeof(ILayoutOrchestrationService)),
            ("page_import", typeof(IPagePackageImportCoordinationService)),
            ("resource_import", typeof(IResourceOrchestrationService)),
            ("script_import", typeof(IScriptOrchestrationService)),
            ("template_import", typeof(ITemplateOrchestrationService)),
            ("common_objects_import", typeof(ICommonObjectCoordinationService)),
            ("app_update", typeof(IPageRenderCacheAggregationService)),
            ("app_delete", typeof(IPageRenderCacheAggregationService)),
            ("app_culture_add", typeof(IPageRenderCacheAggregationService)),
            ("app_culture_delete", typeof(IPageRenderCacheAggregationService)),
            ("layout_add", typeof(IPageRenderCacheAggregationService)),
            ("layout_update", typeof(IPageRenderCacheAggregationService)),
            ("layout_delete", typeof(IPageRenderCacheAggregationService)),
            ("template_add", typeof(IPageRenderCacheAggregationService)),
            ("template_update", typeof(IPageRenderCacheAggregationService)),
            ("template_delete", typeof(IPageRenderCacheAggregationService)),
            ("component_add", typeof(IPageRenderCacheAggregationService)),
            ("component_update", typeof(IPageRenderCacheAggregationService)),
            ("component_delete", typeof(IPageRenderCacheAggregationService)),
            ("resource_add", typeof(IPageRenderCacheAggregationService)),
            ("resource_update", typeof(IPageRenderCacheAggregationService)),
            ("resource_delete", typeof(IPageRenderCacheAggregationService)),
            ("script_add", typeof(IPageRenderCacheAggregationService)),
            ("script_update", typeof(IPageRenderCacheAggregationService)),
            ("script_delete", typeof(IPageRenderCacheAggregationService)),
            ("page_add", typeof(IPageRenderCacheAggregationService)),
            ("page_update", typeof(IPageRenderCacheAggregationService)),
            ("page_delete", typeof(IPageRenderCacheAggregationService)),
            ("content_add", typeof(IPageRenderCacheAggregationService)),
            ("content_update", typeof(IPageRenderCacheAggregationService)),
            ("content_delete", typeof(IPageRenderCacheAggregationService)),
            ("page_info_add", typeof(IPageRenderCacheAggregationService)),
            ("page_info_update", typeof(IPageRenderCacheAggregationService)),
            ("page_info_delete", typeof(IPageRenderCacheAggregationService)),
            ("common_object_add", typeof(IPageRenderCacheAggregationService)),
            ("common_object_update", typeof(IPageRenderCacheAggregationService)),
            ("common_object_delete", typeof(IPageRenderCacheAggregationService)),
            ("common_objects_imported", typeof(IPageRenderCacheAggregationService)),
            ("package_import_complete", typeof(IPageRenderCacheAggregationService)),
            ("render_request", typeof(IUncachedPageRenderOrchestrationService)),
            ("render_request", typeof(IMarkupRenderOrchestrationService)),
            ("render_request", typeof(IUncachedPageRenderOrchestrationService)),
            ("render_tags", typeof(IMarkupRenderTagHandlingProcessingService)),
            ("render_tags", typeof(IMarkupRenderTagHandlingProcessingService)),
            ("render_tags", typeof(IMarkupRenderTagHandlingProcessingService)),
            ("render_tags", typeof(IMarkupRenderTagHandlingProcessingService)),
            ("render_tags", typeof(IMarkupRenderTagHandlingProcessingService)),
            ("render_tags", typeof(IMarkupRenderTagHandlingProcessingService)),
            ("render_tags", typeof(IMarkupRenderTagHandlingProcessingService)),
            ("render_tags", typeof(IMarkupRenderTagHandlingProcessingService)),
            ("render_tags", typeof(IMarkupRenderTagHandlingProcessingService)),
            ("render_tags", typeof(IMarkupRenderTagHandlingProcessingService)),
            ("render_tags", typeof(IMarkupRenderTagHandlingProcessingService)),
            ("app_delete", typeof(IAppOrchestrationService))
        ];

        // When
        eventHubMock.Object.ListenToContentManagementWebEvents();

        // Then
        eventHubMock.Invocations
            .Select(selector: invocation =>
                (
                    EventName: (string)invocation.Arguments[0],
                    ServiceType: invocation.Method.GetGenericArguments()[1]
                ))
            .Should()
            .BeEquivalentTo(expectation: expectedRegistrations);
    }

    [Fact]
    public void ContentManagementEvents_WhenStarted_DoNotRegisterWebCacheBoundaries()
    {
        // Given
        Mock<IEventHub> eventHubMock = new();

        // When
        eventHubMock.Object.ListenToContentManagementEvents();

        // Then
        eventHubMock.Invocations
            .Should()
            .NotContain(predicate: invocation =>
                invocation.Method.GetGenericArguments()[1] ==
                    typeof(IPageRenderCacheAggregationService));

        eventHubMock.Invocations
            .Should()
            .NotContain(predicate: invocation =>
                invocation.Arguments[0] as string == "app_delete" &&
                invocation.Method.GetGenericArguments()[1] ==
                    typeof(IAppOrchestrationService));
    }

    [Fact]
    public void FinalContentManagementEvents_WhenStarted_RegisterOnlyFinalAppDeletion()
    {
        // Given
        Mock<IEventHub> eventHubMock = new();

        // When
        eventHubMock.Object.ListenToFinalContentManagementEvents();

        // Then
        eventHubMock.Invocations
            .Should()
            .ContainSingle(predicate: invocation =>
                invocation.Arguments[0] as string == "app_delete" &&
                invocation.Method.GetGenericArguments()[1] ==
                    typeof(IAppOrchestrationService));
    }

    [Fact]
    public async Task RenderCacheEvents_WhenRaised_UseOwningIdentifiersAsync()
    {
        // Given
        const int appId = 23;
        const int pageId = 17;
        Mock<IEventHub> eventHubMock = new();
        Mock<IPageRenderCacheAggregationService> cacheMock = new(MockBehavior.Strict);

        cacheMock.Setup(expression: service => service.DeleteAppAsync(
                appId: appId,
                fromEvent: true))
            .Returns(value: ValueTask.CompletedTask);

        cacheMock.Setup(expression: service => service.DeletePageAsync(
                pageId: pageId,
                fromEvent: true))
            .Returns(value: ValueTask.CompletedTask);

        eventHubMock.Object.ListenToContentManagementWebEvents();

        // When
        Func<IPageRenderCacheAggregationService, AppCulture, ValueTask> appCultureHandler =
            (Func<IPageRenderCacheAggregationService, AppCulture, ValueTask>)GetHandler(
                eventHubMock: eventHubMock,
                eventName: "app_culture_add",
                serviceType: typeof(IPageRenderCacheAggregationService));

        Func<IPageRenderCacheAggregationService, Layout, ValueTask> layoutHandler =
            (Func<IPageRenderCacheAggregationService, Layout, ValueTask>)GetHandler(
                eventHubMock: eventHubMock,
                eventName: "layout_update",
                serviceType: typeof(IPageRenderCacheAggregationService));

        Func<IPageRenderCacheAggregationService, Template, ValueTask> templateHandler =
            (Func<IPageRenderCacheAggregationService, Template, ValueTask>)GetHandler(
                eventHubMock: eventHubMock,
                eventName: "template_update",
                serviceType: typeof(IPageRenderCacheAggregationService));

        Func<IPageRenderCacheAggregationService, Component, ValueTask> componentHandler =
            (Func<IPageRenderCacheAggregationService, Component, ValueTask>)GetHandler(
                eventHubMock: eventHubMock,
                eventName: "component_update",
                serviceType: typeof(IPageRenderCacheAggregationService));

        Func<IPageRenderCacheAggregationService, Resource, ValueTask> resourceHandler =
            (Func<IPageRenderCacheAggregationService, Resource, ValueTask>)GetHandler(
                eventHubMock: eventHubMock,
                eventName: "resource_update",
                serviceType: typeof(IPageRenderCacheAggregationService));

        Func<IPageRenderCacheAggregationService, Script, ValueTask> scriptHandler =
            (Func<IPageRenderCacheAggregationService, Script, ValueTask>)GetHandler(
                eventHubMock: eventHubMock,
                eventName: "script_update",
                serviceType: typeof(IPageRenderCacheAggregationService));

        Func<IPageRenderCacheAggregationService, Page, ValueTask> pageHandler =
            (Func<IPageRenderCacheAggregationService, Page, ValueTask>)GetHandler(
                eventHubMock: eventHubMock,
                eventName: "page_update",
                serviceType: typeof(IPageRenderCacheAggregationService));

        Func<IPageRenderCacheAggregationService, Content, ValueTask> contentHandler =
            (Func<IPageRenderCacheAggregationService, Content, ValueTask>)GetHandler(
                eventHubMock: eventHubMock,
                eventName: "content_update",
                serviceType: typeof(IPageRenderCacheAggregationService));

        Func<IPageRenderCacheAggregationService, PageInfo, ValueTask> pageInfoHandler =
            (Func<IPageRenderCacheAggregationService, PageInfo, ValueTask>)GetHandler(
                eventHubMock: eventHubMock,
                eventName: "page_info_update",
                serviceType: typeof(IPageRenderCacheAggregationService));

        await appCultureHandler(
            arg1: cacheMock.Object,
            arg2: new AppCulture { AppId = appId });

        await layoutHandler(
            arg1: cacheMock.Object,
            arg2: new Layout { AppId = appId });

        await templateHandler(
            arg1: cacheMock.Object,
            arg2: new Template { AppId = appId });

        await componentHandler(
            arg1: cacheMock.Object,
            arg2: new Component { AppId = appId });

        await resourceHandler(
            arg1: cacheMock.Object,
            arg2: new Resource { AppId = appId });

        await scriptHandler(
            arg1: cacheMock.Object,
            arg2: new Script { AppId = appId });

        await pageHandler(
            arg1: cacheMock.Object,
            arg2: new Page { Id = pageId });

        await contentHandler(
            arg1: cacheMock.Object,
            arg2: new Content { PageId = pageId });

        await pageInfoHandler(
            arg1: cacheMock.Object,
            arg2: new PageInfo { PageId = pageId });

        // Then
        cacheMock.Verify(
            expression: service => service.DeleteAppAsync(
                appId: appId,
                fromEvent: true),
            times: Times.Exactly(callCount: 6));

        cacheMock.Verify(
            expression: service => service.DeletePageAsync(
                pageId: pageId,
                fromEvent: true),
            times: Times.Exactly(callCount: 3));
    }

    [Fact]
    public async Task CommonAndPackageEvents_WhenRaised_UseExpectedCacheScopeAsync()
    {
        // Given
        const int appId = 23;
        const string commonObjectType = "Component";
        Mock<IEventHub> eventHubMock = new();
        Mock<IPageRenderCacheAggregationService> cacheMock = new(MockBehavior.Strict);

        cacheMock.Setup(expression: service =>
                service.InvalidateCommonObjectConsumersAsync(
                    commonObjectType: commonObjectType,
                    fromEvent: true))
            .Returns(value: ValueTask.CompletedTask);

        cacheMock.Setup(expression: service =>
                service.InvalidateCommonCacheAsync(fromEvent: true))
            .Returns(value: ValueTask.CompletedTask);

        cacheMock.Setup(expression: service =>
                service.InvalidatePackageAsync(appId: appId))
            .Returns(value: ValueTask.CompletedTask);

        cacheMock.Setup(expression: service =>
                service.InvalidatePackageAsync(appId: null))
            .Returns(value: ValueTask.CompletedTask);

        eventHubMock.Object.ListenToContentManagementWebEvents();

        // When
        Func<IPageRenderCacheAggregationService, CommonObject, ValueTask> commonObjectHandler =
            (Func<IPageRenderCacheAggregationService, CommonObject, ValueTask>)GetHandler(
                eventHubMock: eventHubMock,
                eventName: "common_object_update",
                serviceType: typeof(IPageRenderCacheAggregationService));

        Func<IPageRenderCacheAggregationService, CommonObject[], ValueTask> commonObjectsHandler =
            (Func<IPageRenderCacheAggregationService, CommonObject[], ValueTask>)GetHandler(
                eventHubMock: eventHubMock,
                eventName: "common_objects_imported",
                serviceType: typeof(IPageRenderCacheAggregationService));

        await commonObjectHandler(
            arg1: cacheMock.Object,
            arg2: new CommonObject { Type = commonObjectType });

        await commonObjectsHandler(
            arg1: cacheMock.Object,
            arg2: []);

        Func<IPageRenderCacheAggregationService, PackageImportEvent, ValueTask> packageHandler =
            (Func<IPageRenderCacheAggregationService, PackageImportEvent, ValueTask>)GetHandler(
                eventHubMock: eventHubMock,
                eventName: "package_import_complete",
                serviceType: typeof(IPageRenderCacheAggregationService));

        await packageHandler(
            arg1: cacheMock.Object,
            arg2: new PackageImportEvent { AppId = appId, Package = new Package() });

        await packageHandler(
            arg1: cacheMock.Object,
            arg2: new PackageImportEvent { AppId = null, Package = new Package() });

        // Then
        cacheMock.VerifyAll();
    }

    private static Delegate GetHandler(
        Mock<IEventHub> eventHubMock,
        string eventName,
        Type serviceType) =>
        (Delegate)eventHubMock.Invocations
            .Single(predicate: invocation =>
                invocation.Arguments[0] as string == eventName &&
                invocation.Method.GetGenericArguments()[1] == serviceType)
            .Arguments[1];
}