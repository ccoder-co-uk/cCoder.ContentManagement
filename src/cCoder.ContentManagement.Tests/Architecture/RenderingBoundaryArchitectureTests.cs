// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Reflection;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.ContentManagement.Rendering.Services.Foundations;
using cCoder.ContentManagement.Services.Foundations.Rendering;
using FluentAssertions;
using Xunit;

namespace cCoder.ContentManagement.Tests.Architecture;

public sealed partial class RenderingBoundaryArchitectureTests
{
    [Fact]
    public void ComponentRenderingFoundation_WhenComposed_ShouldUseFileContentBrokerOnly()
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
            .Equal(elements: typeof(IRenderFileContentBroker));
    }

    [Fact]
    public void TemplateRenderingProcessing_WhenComposed_ShouldNotUseServiceLocatorFoundation()
    {
        // Given
        Assembly assembly = typeof(ComponentRenderService)
            .Assembly;

        // When
        Type templateRenderService = assembly.GetType(
            name: "cCoder.ContentManagement.Services.Foundations.Rendering.TemplateRenderService");

        // Then
        templateRenderService
            .Should()
            .BeNull();
    }
}