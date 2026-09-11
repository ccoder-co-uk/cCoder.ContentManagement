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
    [Theory]
    [InlineData("cCoder.ContentManagement.Exposures.AppManager", typeof(IAppManagerAggregationService))]
    [InlineData("cCoder.ContentManagement.Exposures.ComponentManager", typeof(IComponentOrchestrationService))]
    [InlineData("cCoder.ContentManagement.Exposures.PageManager", typeof(IPageOrchestrationService))]
    [InlineData("cCoder.ContentManagement.Exposures.TemplateManager", typeof(ITemplateOrchestrationService))]
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