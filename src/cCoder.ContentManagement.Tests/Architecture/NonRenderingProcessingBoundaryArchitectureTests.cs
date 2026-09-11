// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Reflection;
using cCoder.ContentManagement.Exposures;
using FluentAssertions;
using Xunit;

namespace cCoder.ContentManagement.Tests.Architecture;

public sealed class NonRenderingProcessingBoundaryArchitectureTests
{
    private static readonly Assembly ContentManagementAssembly =
        typeof(IAppManager).Assembly;

    [Theory]
    [InlineData("AppProcessingService", "IAppService")]
    [InlineData("CommonObjectProcessingService", "ICommonObjectService")]
    [InlineData("JsonProcessingService", "IJsonService")]
    [InlineData("PageProcessingService", "IPageService")]
    [InlineData("PageRenderCacheProcessingService", "IPageRenderCacheService")]
    [InlineData("PageRoleProcessingService", "IPageRoleService")]
    [InlineData("PageRoleImportLookupProcessingService", "IPageRoleService")]
    [InlineData("PageRoleImportPersistenceProcessingService", "IPageRoleService")]
    [InlineData("ResourceProcessingService", "IResourceService")]
    public void ProcessingService_WhenComposed_UsesOnlyItsMatchingFoundation(
        string processingServiceName,
        string foundationServiceName)
    {
        // Given
        Type processingService = ContentManagementAssembly.GetType(
            name: $"cCoder.ContentManagement.Services.Processings.{processingServiceName}");

        // When
        Type[] dependencies = processingService
            .GetConstructors(
                bindingAttr: BindingFlags.Instance
                    | BindingFlags.NonPublic
                    | BindingFlags.Public)
            .Single()
            .GetParameters()
            .Select(selector: parameter => parameter.ParameterType)
            .ToArray();

        // Then
        dependencies.Should().ContainSingle();
        dependencies.Single().Name.Should().Be(foundationServiceName);
    }
}
