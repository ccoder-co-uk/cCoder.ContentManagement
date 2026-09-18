// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Reflection;
using cCoder.ContentManagement.Services.Foundations.Rendering;
using FluentAssertions;
using Xunit;

namespace cCoder.ContentManagement.Tests.Architecture;

public sealed partial class CombinedRenderingArchitectureTests
{
    private static readonly Assembly ContentManagementAssembly =
        typeof(ComponentRenderService)
            .Assembly;

    [Fact]
    public void MarkupRendering_WhenComposed_DoesNotHideProcessingHandlersBehindBroker()
    {
        // Given
        Type renderBroker = ContentManagementAssembly.GetType(
            name: "cCoder.ContentManagement.Rendering.Brokers.RenderBroker");

        // When
        Type[] dependencies = renderBroker is null
            ? []
            : GetConstructorDependencies(type: renderBroker);

        // Then
        dependencies.Should()
            .NotContain(predicate: dependency =>
            dependency.IsGenericType
            && dependency.GetGenericArguments()
                .Any(predicate: argument =>
                argument.Name == "ITagHandlingProcessingService"));

        renderBroker.Should()
            .BeNull(
                because: "the obsolete render exposure loop was retired");
    }

    [Fact]
    public void PageRendering_WhenComposed_DoesNotUseServiceProviderBroker()
    {
        // Given
        Type pageRenderService = ContentManagementAssembly.GetType(
            name: "cCoder.ContentManagement.Services.Foundations.Rendering.PageRenderService");

        // When
        Type[] dependencies = GetConstructorDependencies(type: pageRenderService);

        // Then
        dependencies.Should()
            .ContainSingle(predicate: dependency =>
                dependency.Name == "IJsonBroker");

        dependencies.Should()
            .NotContain(predicate: dependency =>
                dependency.Name == "IRenderBroker");

        dependencies.Should()
            .NotContain(predicate: dependency =>
                dependency.Name == "IServiceProviderBroker");
    }

    [Fact]
    public void PageRendering_WhenComposed_UsesJsonBoundaryForSerializationAndFingerprinting()
    {
        // Given
        Type pageRenderService = ContentManagementAssembly.GetType(
            name: "cCoder.ContentManagement.Services.Foundations.Rendering.PageRenderService");

        // When
        Type[] dependencies = GetConstructorDependencies(type: pageRenderService);

        // Then
        dependencies.Should()
            .ContainSingle(predicate: dependency =>
                dependency.Name == "IJsonBroker",
                because: "JSON serialization and its deterministic fingerprint belong to the JSON boundary");

        dependencies.Should()
            .NotContain(predicate: dependency =>
                dependency.Name == "IRenderingUtilityBroker");
    }

    [Fact]
    public void RenderingUtilities_WhenComposed_AreOwnedByNormalRenderingBoundaries()
    {
        // Given
        Type renderingUtilityBroker = ContentManagementAssembly.GetType(
            name: "cCoder.ContentManagement.Brokers.Rendering.RenderingUtilityBroker");

        Type renderAggregationService = ContentManagementAssembly.GetType(
            name: "cCoder.ContentManagement.Services.Aggregations.RenderAggregationService");

        Type[] renderingFoundationTypes =
        [
            ContentManagementAssembly.GetType(
                name: "cCoder.ContentManagement.Services.Foundations.Rendering.ComponentRenderService"),
            ContentManagementAssembly.GetType(
                name: "cCoder.ContentManagement.Services.Foundations.Rendering.TemplateRenderService"),
            ContentManagementAssembly.GetType(
                name: "cCoder.ContentManagement.Rendering.Services.Foundations.MarkupRenderService")
        ];

        // When
        Type[] aggregationDependencies = GetConstructorDependencies(
            type: renderAggregationService);

        Type[] renderingFoundationDependencies = renderingFoundationTypes
            .SelectMany(selector: GetConstructorDependencies)
            .ToArray();

        // Then
        renderingUtilityBroker.Should()
            .BeNull(
                because: "HTML encoding and property discovery have distinct normal rendering boundaries");

        aggregationDependencies.Should()
            .ContainSingle(predicate: dependency =>
                dependency.Name == "IRenderDataOrchestrationService");

        aggregationDependencies.Should()
            .NotContain(predicate: dependency =>
                dependency.Name.EndsWith(
                    value: "Broker",
                    comparisonType: StringComparison.Ordinal));

        renderingFoundationDependencies.Should()
            .NotContain(predicate: dependency =>
                dependency.Name == "IRenderingUtilityBroker");
    }

    [Fact]
    public void TagHandling_WhenComposed_IsOwnedByFocusedProcessingOperations()
    {
        // Given
        Type[] contentManagementTypes = ContentManagementAssembly.GetTypes();

        // When
        Type markupRenderProcessingContract = contentManagementTypes
            .Single(predicate: type =>
                type.Name == "IMarkupRenderTagHandlingProcessingService");

        string[] processingOperations = markupRenderProcessingContract
            .GetMethods()
            .Where(predicate: method => method.Name.EndsWith(
                value: "TagHandlingOperationAsync",
                comparisonType: StringComparison.Ordinal))
            .Select(selector: method => method.Name)
            .ToArray();

        // Then
        processingOperations.Should()
            .BeEquivalentTo(
            expectation:
            [
                "RenderComponentTagHandlingOperationAsync",
                "RenderContentTagHandlingOperationAsync",
                "RenderCultureLinkTagHandlingOperationAsync",
                "RenderDmsTagHandlingOperationAsync",
                "RenderExecuteTagHandlingOperationAsync",
                "RenderMetadataTagHandlingOperationAsync",
                "RenderNavigationTagHandlingOperationAsync",
                "RenderReplacementTagHandlingOperationAsync",
                "RenderResourceTagHandlingOperationAsync",
                "RenderScriptTagHandlingOperationAsync",
                "RenderStyleTagHandlingOperationAsync"
            ],
            because: "render_tags must fan out into one focused operation per tag family");
    }

    [Fact]
    public void Rendering_WhenComposed_UsesEventsForCacheMissWork()
    {
        // Given
        Type renderAggregationService = ContentManagementAssembly.GetType(
            name: "cCoder.ContentManagement.Services.Aggregations.RenderAggregationService");

        Type renderEventBroker = ContentManagementAssembly.GetType(
            name: "cCoder.ContentManagement.Brokers.Events.RenderEventBroker");

        Type renderEventProcessingService = ContentManagementAssembly.GetType(
            name: "cCoder.ContentManagement.Services.Processings.HttpPageRenderOperationEventProcessingService");

        // When
        Type[] aggregationDependencies = GetConstructorDependencies(
            type: renderAggregationService);

        Type[] eventProcessingDependencies = GetConstructorDependencies(
            type: renderEventProcessingService);

        // Then
        aggregationDependencies
            .Should()
            .ContainSingle(
            predicate: dependency =>
                dependency.Name == "IRenderEventOrchestrationService");

        aggregationDependencies
            .Should()
            .NotContain(
            predicate: dependency =>
                dependency.Name == "IUncachedPageRenderOrchestrationService");

        renderEventBroker.Should()
            .NotBeNull(
                because: "render events must cross the IEventHub boundary through a broker");

        eventProcessingDependencies.Should()
            .ContainSingle(predicate: dependency =>
                dependency.Name == "IHttpPageRenderOperationEventService");
    }

    [Fact]
    public void MarkupReplacement_WhenModelled_IsNotADependencyElement()
    {
        // Given
        Type oldDependency = ContentManagementAssembly.GetType(
            name: "cCoder.ContentManagement.Dependencies.ReplacementDependency");

        // When
        Type markupReplacement = ContentManagementAssembly.GetType(
            name: "cCoder.ContentManagement.Models.PageRendering.MarkupReplacement");

        // Then
        oldDependency.Should()
            .BeNull();

        markupReplacement.Should()
            .NotBeNull();
    }

    [Fact]
    public void CachedPageRendering_WhenComposed_UsesMatchingFoundation()
    {
        // Given
        Type cachedPageRenderProcessing = ContentManagementAssembly.GetType(
            name: "cCoder.ContentManagement.Services.Processings.CachedPageRenderProcessingService");

        // When
        Type[] dependencies = GetConstructorDependencies(
            type: cachedPageRenderProcessing);

        // Then
        dependencies.Should()
            .ContainSingle(predicate: dependency =>
            dependency.Name == "ICachedPageRenderService");
    }

    private static Type[] GetConstructorDependencies(Type type) =>
        type.GetConstructors(
            bindingAttr: BindingFlags.Instance
                | BindingFlags.NonPublic
                | BindingFlags.Public)
        .Single()
        .GetParameters()
        .Select(selector: parameter => parameter.ParameterType)
        .ToArray();
}