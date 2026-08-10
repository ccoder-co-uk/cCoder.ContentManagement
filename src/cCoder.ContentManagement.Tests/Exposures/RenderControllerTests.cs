// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Loggings;
using cCoder.ContentManagement.Exposures;
using cCoder.ContentManagement.Exposures.Controllers;
using cCoder.ContentManagement.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Exposures;

public sealed partial class RenderControllerTests
{
    [Fact]
    public async Task ShouldReturnRenderedTemplateContentAsync()
    {
        // Given
        Mock<IRenderer> renderer = new();
        object model = new { Name = "Paul" };

        renderer.Setup(expression: service =>
                service.RenderTemplateRenderResultAsync(
                    name: "Welcome",
                    model: model))
            .Returns(value: ValueTask.FromResult<RenderResult>(
                result: new TemplateRenderResult { Content = "template" }));

        RenderController controller = new(
            renderer: renderer.Object,
            loggingBroker: Mock.Of<ILoggingBroker>());

        // When
        IActionResult actionResult = await controller.PostTemplate(
            name: "Welcome",
            model: model);

        // Then
        ContentResult result = actionResult
            .Should()
            .BeOfType<ContentResult>()
            .Subject;

        result.Content
            .Should()
            .Be(expected: "template");

        result.ContentType
            .Should()
            .Be(expected: "text/plain");
    }

    [Fact]
    public async Task ShouldReturnRenderedComponentContentAsync()
    {
        // Given
        Mock<IRenderer> renderer = new();

        renderer.Setup(expression: service =>
                service.RenderComponentRenderResultAsync(name: "Hero"))
            .Returns(value: ValueTask.FromResult<RenderResult>(
                result: new ComponentRenderResult { Content = "component" }));

        RenderController controller = new(
            renderer: renderer.Object,
            loggingBroker: Mock.Of<ILoggingBroker>());

        // When
        IActionResult actionResult = await controller.GetComponent(name: "Hero");

        // Then
        OkObjectResult result = actionResult
            .Should()
            .BeOfType<OkObjectResult>()
            .Subject;

        result.Value
            .Should()
            .Be(expected: "component");
    }
}