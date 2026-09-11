// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Reflection;
using cCoder.ContentManagement.Services.Foundations.Rendering;
using FluentAssertions;
using Xunit;

namespace cCoder.ContentManagement.Tests.Architecture;

public sealed class CombinedRenderingArchitectureTests
{
    private static readonly Assembly ContentManagementAssembly =
        typeof(PageRenderService).Assembly;

    [Fact]
    public void MarkupRendering_WhenComposed_DoesNotHideProcessingHandlersBehindBroker()
    {
        // Given
        Type renderBroker = ContentManagementAssembly.GetType(
            name: "cCoder.ContentManagement.Rendering.Brokers.RenderBroker");

        // When
        Type[] dependencies = GetConstructorDependencies(type: renderBroker);

        // Then
        dependencies.Should().NotContain(dependency =>
            dependency.IsGenericType
            && dependency.GetGenericArguments().Any(argument =>
                argument.Name == "ITagHandlingProcessingService"));
    }

    [Fact]
    public void PageRendering_WhenComposed_DoesNotUseServiceProviderBroker()
    {
        // Given
        Type pageRenderService = typeof(PageRenderService);

        // When
        Type[] dependencies = GetConstructorDependencies(type: pageRenderService);

        // Then
        dependencies.Should().NotContain(dependency =>
            dependency.Name == "IServiceProviderBroker");
    }

    [Fact]
    public void TagHandling_WhenComposed_IsOwnedByMarkupFoundation()
    {
        // Given
        Type[] contentManagementTypes = ContentManagementAssembly.GetTypes();

        // When
        Type[] processingHandlers = contentManagementTypes
            .Where(type => type.Name.EndsWith(
                value: "TagHandlingProcessingService",
                comparisonType: StringComparison.Ordinal))
            .ToArray();

        // Then
        processingHandlers.Should().BeEmpty();
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
        oldDependency.Should().BeNull();
        markupReplacement.Should().NotBeNull();
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
        dependencies.Should().ContainSingle(dependency =>
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
