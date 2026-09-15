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
    public static TheoryData<string> RenderingFoundationTypeNames =>
        new()
        {
            "cCoder.ContentManagement.Rendering.Services.Foundations.MarkupRenderService",
            "cCoder.ContentManagement.Services.Foundations.Rendering.ComponentRenderService",
            "cCoder.ContentManagement.Services.Foundations.Rendering.TemplateRenderService"
        };

    [Theory]
    [MemberData(nameof(RenderingFoundationTypeNames))]
    public void RenderingFoundation_WhenComposed_UsesOneContentRenderBroker(
        string foundationTypeName)
    {
        // Given
        Assembly assembly = typeof(ComponentRenderService).Assembly;

        Type foundationType = assembly.GetType(name: foundationTypeName);

        // When
        Type[] ordinaryBrokerTypes = foundationType
            .GetConstructors(bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            .Single()
            .GetParameters()
            .Select(selector: parameter => parameter.ParameterType)
            .Where(predicate: dependencyType =>
                dependencyType.Name.EndsWith(
                    value: "Broker",
                    comparisonType: StringComparison.Ordinal)
                && dependencyType.Name is not "IJsonBroker"
                && dependencyType.Name is not "ILoggingBroker"
                && dependencyType.Name is not "IRegularExpressionBroker"
                && dependencyType.Name is not "IRenderingUtilityBroker")
            .ToArray();

        // Then
        ordinaryBrokerTypes.Should()
            .ContainSingle(predicate: dependencyType =>
                dependencyType.Name == "IContentRenderBroker");
    }

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