// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Reflection;
using cCoder.ContentManagement.Services.Foundations.Rendering;
using FluentAssertions;
using Xunit;

namespace cCoder.ContentManagement.Tests.Architecture;

public sealed partial class RenderingBoundaryArchitectureTests
{
    [Fact]
    public void ComponentRenderingFoundation_WhenComposed_ShouldOwnBrokerOperations()
    {
        // Given
        Type componentRenderServiceType = typeof(ComponentRenderService)
            .Assembly
            .GetType(name: "cCoder.ContentManagement.Services.Foundations.Rendering.ComponentRenderService");

        // When
        Type[] dependencyTypes = componentRenderServiceType
            .GetConstructors(bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            .Single()
            .GetParameters()
            .Select(selector: parameter => parameter.ParameterType)
            .ToArray();

        // Then
        dependencyTypes.Should()
            .OnlyContain(predicate: dependencyType =>
                dependencyType.Name.EndsWith(
                    value: "Broker",
                    comparisonType: StringComparison.Ordinal));
    }

    [Fact]
    public void TemplateRenderingProcessing_WhenComposed_ShouldUseMatchingFoundation()
    {
        // Given
        Assembly assembly = typeof(ComponentRenderService)
            .Assembly;

        // When
        Type templateRenderProcessingService = assembly.GetType(
            name: "cCoder.ContentManagement.Services.Processings.TemplateRenderProcessingService");

        Type[] dependencyTypes = templateRenderProcessingService
            .GetConstructors(bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            .Single()
            .GetParameters()
            .Select(selector: parameter => parameter.ParameterType)
            .ToArray();

        // Then
        dependencyTypes.Should()
            .ContainSingle(predicate: dependencyType =>
                dependencyType.Name == "ITemplateRenderService");
    }
}
