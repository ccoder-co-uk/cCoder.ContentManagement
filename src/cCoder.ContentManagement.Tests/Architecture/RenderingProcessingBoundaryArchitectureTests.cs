// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Reflection;
using System.Runtime.CompilerServices;
using cCoder.ContentManagement.Services.Foundations.Rendering;
using FluentAssertions;
using Xunit;

namespace cCoder.ContentManagement.Tests.Architecture;

public sealed partial class RenderingProcessingBoundaryArchitectureTests
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

    public static TheoryData<string> RenderingProcessingSources =>
        new()
        {
            "Services/Processings/ComponentRenderProcessingService.cs",
            "Services/Processings/TemplateRenderProcessingService.cs",
            "Services/Processings/PageRenderProcessingService.cs",
            "Services/Processings/PageRenderCacheProcessingService.cs",
            "Services/Processings/PageRendering/MarkupRenderProcessingService.cs"
        };

    public static TheoryData<string> MutableTextProcessingSources =>
        new()
        {
            "Services/Processings/ComponentRenderProcessingService.cs",
            "Services/Processings/TemplateRenderProcessingService.cs"
        };

    private static readonly string[] EnumerableOperations =
    [
        ".Any(",
        ".First(",
        ".FirstOrDefault(",
        ".GroupBy(",
        ".Last(",
        ".OrderBy(",
        ".OrderByDescending(",
        ".Select(",
        ".SelectMany(",
        ".Single(",
        ".SingleOrDefault(",
        ".Take(",
        ".ToArray(",
        ".ToDictionary(",
        ".ToList(",
        ".Where("
    ];

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
        dependencies.Should()
            .ContainSingle(predicate: dependency =>
                dependency.Name == foundationContractName);

        dependencies.Should()
            .NotContain(predicate: dependency =>
                IsForbiddenProcessingDependency(type: dependency));
    }

    [Theory]
    [MemberData(nameof(JsonModelLeakingProcessingSources))]
    public void RenderingProcessing_WhenInspectingJson_DoesNotUseExternalJsonModels(
        string relativeSourcePath)
    {
        // Given
        string sourcePath = Path.GetFullPath(
            path: Path.Combine(
                paths:
                [
                    Path.GetDirectoryName(path: GetThisFilePath()),
                    "..",
                    "..",
                    "cCoder.ContentManagement",
                    relativeSourcePath
                ]));

        // When
        string source = File.ReadAllText(path: sourcePath);

        // Then
        source.Should()
            .NotContain(unexpected: "System.Text.Json");

        source.Should()
            .NotContain(unexpected: "JsonElement");
    }

    [Theory]
    [MemberData(nameof(RenderingProcessingSources))]
    public void RenderingProcessing_WhenImplemented_DoesNotUseCollectionsMarshal(
        string relativeSourcePath)
    {
        // Given
        string source = ReadRenderingSource(relativeSourcePath: relativeSourcePath);

        // When
        bool usesCollectionsMarshal = source.Contains(
            value: "CollectionsMarshal",
            comparisonType: StringComparison.Ordinal);

        // Then
        usesCollectionsMarshal.Should()
            .BeFalse(
                because: "processing logic should use normal collection operations rather than runtime interop helpers");
    }

    [Theory]
    [MemberData(nameof(RenderingProcessingSources))]
    public void RenderingProcessing_WhenImplemented_DoesNotUseEnumerableExtensions(
        string relativeSourcePath)
    {
        // Given
        string source = ReadRenderingSource(relativeSourcePath: relativeSourcePath);

        // When
        string[] usedOperations = EnumerableOperations
            .Where(predicate: operation => source.Contains(
                value: operation,
                comparisonType: StringComparison.Ordinal))
            .ToArray();

        // Then
        usedOperations.Should()
            .BeEmpty(
                because: "render processing should build its in-memory results directly without depending on Enumerable helpers");
    }

    [Theory]
    [MemberData(nameof(MutableTextProcessingSources))]
    public void RenderingProcessing_WhenBuildingMarkup_DoesNotUseStringBuilder(
        string relativeSourcePath)
    {
        // Given
        string source = ReadRenderingSource(relativeSourcePath: relativeSourcePath);

        // When
        bool usesStringBuilder = source.Contains(
            value: "StringBuilder",
            comparisonType: StringComparison.Ordinal);

        // Then
        usesStringBuilder.Should()
            .BeFalse(
                because: "processing logic should compose strings through its rendering foundation rather than an external mutable text component");
    }

    [Theory]
    [MemberData(nameof(RenderingProcessingSources))]
    public void RenderingProcessing_WhenInspectingRuntimeTypes_UsesItsFoundation(
        string relativeSourcePath)
    {
        // Given
        string source = ReadRenderingSource(relativeSourcePath: relativeSourcePath);

        // When
        bool directlyInspectsRuntimeTypes = source.Contains(
            value: ".GetType()",
            comparisonType: StringComparison.Ordinal);

        // Then
        directlyInspectsRuntimeTypes.Should()
            .BeFalse(
                because: "runtime type inspection is an external concern owned by the rendering broker boundary");
    }

    [Theory]
    [MemberData(nameof(RenderingProcessingSources))]
    public void RenderingProcessing_WhenConvertingRuntimeValues_UsesItsFoundation(
        string relativeSourcePath)
    {
        // Given
        string source = ReadRenderingSource(relativeSourcePath: relativeSourcePath);

        // When
        string[] directRuntimeConversions =
        [
            "(object)model",
            "BuildThemeReplacements<T>",
            "BuildIEnumerableThemeReplacements<T>",
            "BuildJObjectThemeReplacements<T>",
            "model.ToString()",
            "model?.ToString()",
            "value.ToString()",
            "value?.ToString()",
            "property.Value.ToString()",
            "token.Value.ToString()",
            "dynamicModel[key]?.ToString()"
        ];

        string[] usedConversions = directRuntimeConversions
            .Where(predicate: conversion => source.Contains(
                value: conversion,
                comparisonType: StringComparison.Ordinal))
            .ToArray();

        // Then
        usedConversions.Should()
            .BeEmpty(
                because: "runtime value conversion is an external concern owned by the rendering broker boundary");
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

    private static string ReadRenderingSource(string relativeSourcePath)
    {
        string sourcePath = Path.GetFullPath(
            path: Path.Combine(
                paths:
                [
                    Path.GetDirectoryName(path: GetThisFilePath()),
                    "..",
                    "..",
                    "cCoder.ContentManagement",
                    relativeSourcePath
                ]));

        return File.ReadAllText(path: sourcePath);
    }

    private static string GetThisFilePath(
        [CallerFilePath] string filePath = "") =>
        filePath;
}