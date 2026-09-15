// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Reflection;
using cCoder.ContentManagement.Exposures;
using FluentAssertions;
using Xunit;

namespace cCoder.ContentManagement.Tests.Architecture;

public sealed partial class NonRenderingProcessingBoundaryArchitectureTests
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
        dependencies.Should()
            .ContainSingle();

        dependencies
            .Single()
            .Name.Should()
            .Be(expected: foundationServiceName);
    }

    [Fact]
    public void CurrentAppOrchestration_WhenComposed_UsesAppAndHttpContextFoundations()
    {
        // Given
        Type processingService = ContentManagementAssembly.GetType(
            name: "cCoder.ContentManagement.Services.Orchestrations.CurrentAppOrchestrationService");

        // When
        string[] dependencyNames = processingService
            .GetConstructors(
                bindingAttr: BindingFlags.Instance
                    | BindingFlags.NonPublic
                    | BindingFlags.Public)
            .Single()
            .GetParameters()
            .Select(selector: parameter => parameter.ParameterType.Name)
            .ToArray();

        // Then
        dependencyNames.Should()
            .BeEquivalentTo(expectation: ["IAppService", "IHttpContextService"]);
    }

    [Fact]
    public void AppFoundation_WhenExposed_UsesOnlyAppOperationContract()
    {
        // Given
        Type appService = ContentManagementAssembly.GetType(
            name: "cCoder.ContentManagement.Services.Foundations.Storages.IAppService");

        // When
        MethodInfo[] methods = appService.GetMethods();

        // Then
        methods.Should()
            .NotBeEmpty();

        methods.SelectMany(selector: method => method.GetParameters())
            .Should()
            .OnlyContain(predicate: parameter => parameter.ParameterType.Name == "AppOperation");

        methods.Should()
            .OnlyContain(predicate: method =>
                method.ReturnType.Name == "AppOperation"
                || (method.ReturnType.GenericTypeArguments.Length == 1
                    && method.ReturnType.GenericTypeArguments[0].Name == "AppOperation"));
    }

    [Fact]
    public void AppFoundation_WhenComposed_UsesOnlyItsAppBroker()
    {
        // Given
        Type appService = ContentManagementAssembly.GetType(
            name: "cCoder.ContentManagement.Services.Foundations.Storages.AppService");

        // When
        Type[] dependencies = appService
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
            .ContainSingle();

        dependencies.Single().Name.Should()
            .Be(expected: "IAppBroker");
    }
}