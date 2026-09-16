// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Reflection;
using cCoder.ContentManagement.Exposures;
using cCoder.ContentManagement.Services.Aggregations;
using cCoder.ContentManagement.Services.Orchestrations;
using FluentAssertions;
using Xunit;

namespace cCoder.ContentManagement.Tests.Architecture;

public sealed partial class ExposureBoundaryArchitectureTests
{
    [Fact]
    public void HttpAndEventExposures_WhenComposed_DoNotDependOnLocalManagerExposures()
    {
        // Given
        Assembly assembly = typeof(IAppManager)
            .Assembly;

        Type[] httpAndEventExposureTypes = assembly
            .GetTypes()
            .Where(predicate: type =>
                type.Namespace is not null
                && (type.Namespace.EndsWith(
                        value: ".Exposures.Controllers",
                        comparisonType: StringComparison.Ordinal)
                    || type.Namespace.EndsWith(
                        value: ".Exposures.EventHandlers",
                        comparisonType: StringComparison.Ordinal)))
            .ToArray();

        // When
        string[] invalidDependencies = httpAndEventExposureTypes
            .SelectMany(selector: type => type
                .GetConstructors(bindingAttr:
                    BindingFlags.Instance
                    | BindingFlags.NonPublic
                    | BindingFlags.Public)
                .SelectMany(selector: constructor => constructor
                    .GetParameters()
                    .Select(selector: parameter => new
                    {
                        Exposure = type,
                        Dependency = parameter.ParameterType
                    })))
            .Where(predicate: dependency =>
                dependency.Dependency.Namespace ==
                    "cCoder.ContentManagement.Exposures"
                && dependency.Dependency.Name.EndsWith(
                    value: "Manager",
                    comparisonType: StringComparison.Ordinal))
            .Select(selector: selector =>
                $"{selector.Exposure.Name} -> {selector.Dependency.Name}")
            .Order()
            .ToArray();

        // Then
        invalidDependencies.Should()
            .BeEmpty(
                because: "one exposure must consume the underlying service contract rather than another local exposure");
    }

    [Fact]
    public void Brokers_WhenComposed_DoNotReceiveCoreDataContextDirectly()
    {
        // Given
        Assembly assembly = typeof(IAppManager)
            .Assembly;

        Type[] brokerTypes = assembly
            .GetTypes()
            .Where(predicate: type =>
                type.Namespace is not null
                && type.Namespace.Contains(
                    value: ".Brokers",
                    comparisonType: StringComparison.Ordinal)
                && type.Name.EndsWith(
                    value: "Broker",
                    comparisonType: StringComparison.Ordinal)
                && type.IsClass)
            .ToArray();

        // When
        string[] directContextDependencies = brokerTypes
            .SelectMany(selector: type => type
                .GetConstructors(bindingAttr:
                    BindingFlags.Instance
                    | BindingFlags.NonPublic
                    | BindingFlags.Public)
                .SelectMany(selector: constructor => constructor
                    .GetParameters()
                    .Where(predicate: parameter =>
                        parameter.ParameterType.FullName ==
                            "cCoder.Data.CoreDataContext")
                    .Select(selector: parameter => type.Name)))
            .Order()
            .ToArray();

        // Then
        directContextDependencies.Should()
            .BeEmpty(
                because: "brokers must create their contexts through ICoreContextFactory rather than receive a scoped context directly");
    }

    [Fact]
    public void ServiceContracts_WhenComposed_DoNotInheritLocalManagerExposures()
    {
        // Given
        Assembly assembly = typeof(IAppManager)
            .Assembly;

        Type[] serviceContracts = assembly
            .GetTypes()
            .Where(predicate: type =>
                type.IsInterface
                && type.Namespace is not null
                && type.Namespace.Contains(
                    value: ".Services.",
                    comparisonType: StringComparison.Ordinal))
            .ToArray();

        // When
        string[] invalidBaseContracts = serviceContracts
            .SelectMany(selector: serviceContract => serviceContract
                .GetInterfaces()
                .Where(predicate: baseContract =>
                    baseContract.Namespace ==
                        "cCoder.ContentManagement.Exposures"
                    && baseContract.Name.EndsWith(
                        value: "Manager",
                        comparisonType: StringComparison.Ordinal))
                .Select(selector: baseContract =>
                    $"{serviceContract.Name} -> {baseContract.Name}"))
            .Order()
            .ToArray();

        // Then
        invalidBaseContracts.Should()
            .BeEmpty(
                because: "a service contract must own its operations rather than inherit them from an exposure contract");
    }

    [Theory]
    [InlineData("cCoder.ContentManagement.Exposures.AppManager", typeof(IAppManagerAggregationService))]
    [InlineData("cCoder.ContentManagement.Exposures.ComponentManager", typeof(IComponentOrchestrationService))]
    [InlineData("cCoder.ContentManagement.Exposures.PageManager", typeof(IPageOrchestrationService))]
    [InlineData("cCoder.ContentManagement.Exposures.TemplateManager", typeof(ITemplateManagerAggregationService))]
    public void Manager_WhenComposed_ShouldUseOnlyItsOrchestration(
        string managerTypeName,
        Type expectedDependencyType)
    {
        // Given
        Type managerType = typeof(IAppManager)
            .Assembly.GetType(name: managerTypeName);

        // When
        Type[] dependencyTypes = managerType
            .GetConstructors(bindingAttr:
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            .Single()
            .GetParameters()
            .Select(selector: parameter => parameter.ParameterType)
            .ToArray();

        // Then
        dependencyTypes.Should()
            .Equal(elements: expectedDependencyType);
    }

    [Fact]
    public void EventRegistration_WhenComposed_ShouldNotUseFoundationService()
    {
        // Given
        Assembly assembly = typeof(IAppManager)
            .Assembly;

        // When
        Type foundationEventHandlerType = assembly.GetType(
            name: "cCoder.ContentManagement.Services.Foundations.Events.EventHandlerService");

        // Then
        foundationEventHandlerType.Should()
            .BeNull();
    }
}