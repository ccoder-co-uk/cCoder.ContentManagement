// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Reflection;
using System.Runtime.CompilerServices;
using cCoder.ContentManagement.Services.Foundations.Rendering;
using FluentAssertions;
using Xunit;

namespace cCoder.ContentManagement.Tests.Architecture;

public sealed class RenderingProcessingBoundaryArchitectureTests
{
    private static readonly Assembly ContentManagementAssembly =
        typeof(ComponentRenderService).Assembly;

    public static TheoryData<string, string> ProcessingServices =>
        new()
        {
            {
                "cCoder.ContentManagement.Services.Processings.ComponentRenderProcessingService",
                "IComponentRenderService"
            },
            {
                "cCoder.ContentManagement.Services.Processings.TemplateRenderProcessingService",
                "ITemplateRenderService"
            },
            {
                "cCoder.ContentManagement.Services.Processings.PageRenderProcessingService",
                "IPageRenderService"
            },
            {
                "cCoder.ContentManagement.Rendering.Services.Processings.MarkupRenderProcessingService",
                "IMarkupRenderService"
            }
        };

    public static TheoryData<string> JsonModelLeakingProcessingSources =>
        new()
        {
            "Services/Processings/TemplateRenderProcessingService.cs",
            "Services/Processings/PageRendering/MarkupRenderProcessingService.cs"
        };

    [Theory]
    [MemberData(nameof(ProcessingServices))]
    public void RenderingProcessing_WhenComposed_UsesOnlyMatchingFoundation(
        string processingTypeName,
        string foundationContractName)
    {
        // Given
        Type processingType = ContentManagementAssembly.GetType(
            name: processingTypeName);

        // When
        Type[] dependencies = processingType
            .GetConstructors(
                bindingAttr: BindingFlags.Instance
                    | BindingFlags.NonPublic
                    | BindingFlags.Public)
            .Single()
            .GetParameters()
            .Select(selector: parameter => parameter.ParameterType)
            .ToArray();

        // Then
        dependencies.Should().ContainSingle(dependency =>
            dependency.Name == foundationContractName);

        dependencies.Should().NotContain(dependency =>
            IsForbiddenProcessingDependency(type: dependency));
    }

    [Theory]
    [MemberData(nameof(JsonModelLeakingProcessingSources))]
    public void RenderingProcessing_WhenInspectingJson_DoesNotUseExternalJsonModels(
        string relativeSourcePath)
    {
        // Given
        string sourcePath = Path.GetFullPath(path: Path.Combine(
            Path.GetDirectoryName(path: GetThisFilePath()),
            "..",
            "..",
            "cCoder.ContentManagement",
            relativeSourcePath));

        // When
        string source = File.ReadAllText(path: sourcePath);

        // Then
        source.Should().NotContain(
            unexpected: "System.Text.Json");

        source.Should().NotContain(
            unexpected: "JsonElement");
    }

    private static bool IsForbiddenProcessingDependency(Type type)
    {
        string namespaceName = type.Namespace ?? string.Empty;

        return namespaceName.Contains(
            value: ".Brokers",
            comparisonType: StringComparison.Ordinal)
            || namespaceName.Contains(
                value: ".Exposures",
                comparisonType: StringComparison.Ordinal)
            || namespaceName.Contains(
                value: ".HttpExposures",
                comparisonType: StringComparison.Ordinal);
    }

    private static string GetThisFilePath(
        [CallerFilePath] string filePath = "") =>
        filePath;
}