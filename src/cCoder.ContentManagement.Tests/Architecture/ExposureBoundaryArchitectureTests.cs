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

public sealed class ExposureBoundaryArchitectureTests
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
        Type managerType = typeof(IAppManager).Assembly.GetType(managerTypeName);

        Type[] dependencyTypes = managerType
            .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            .Single()
            .GetParameters()
            .Select(parameter => parameter.ParameterType)
            .ToArray();

        dependencyTypes.Should().Equal(expectedDependencyType);
    }

    [Fact]
    public void EventRegistration_WhenComposed_ShouldNotUseFoundationService()
    {
        Type foundationEventHandlerType = typeof(IAppManager).Assembly.GetType(
            "cCoder.ContentManagement.Services.Foundations.Events.EventHandlerService");

        foundationEventHandlerType.Should().BeNull();
    }
}