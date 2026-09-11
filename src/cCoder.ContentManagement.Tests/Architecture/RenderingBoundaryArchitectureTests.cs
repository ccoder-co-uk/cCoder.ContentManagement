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

public sealed class RenderingBoundaryArchitectureTests
{
    [Fact]
    public void ComponentRenderingFoundation_WhenComposed_ShouldUseFileContentBrokerOnly()
    {
        Type componentRenderServiceType = typeof(PageRenderService).Assembly
            .GetType("cCoder.ContentManagement.Services.Foundations.Rendering.ComponentRenderService");

        Type[] dependencyTypes = componentRenderServiceType
            .GetConstructors(bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            .Single()
            .GetParameters()
            .Select(selector: parameter => parameter.ParameterType)
            .ToArray();

        dependencyTypes.Should().Equal(typeof(IRenderFileContentBroker));
    }

    [Fact]
    public void TemplateRenderingProcessing_WhenComposed_ShouldNotUseServiceLocatorFoundation()
    {
        Assembly assembly = typeof(PageRenderService).Assembly;

        assembly.GetType(name: "cCoder.ContentManagement.Services.Foundations.Rendering.TemplateRenderService")
            .Should().BeNull();
    }
}
