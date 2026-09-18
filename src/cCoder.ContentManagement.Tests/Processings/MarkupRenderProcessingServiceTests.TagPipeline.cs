// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.ContentManagement.Models.PageRendering;
using cCoder.ContentManagement.Rendering.Brokers;
using cCoder.ContentManagement.Rendering.Services.Foundations;
using cCoder.ContentManagement.Rendering.Services.Processings;
using FluentAssertions;
using Moq;
using Xunit;
using cCoder.ContentManagement.Tests.Brokers.Rendering;

namespace cCoder.ContentManagement.Tests.Processings;

public sealed partial class MarkupRenderProcessingServiceTagPipelineTests
{
    [Fact]
    public void ShouldApplyTagHandlersInTheirDefinedOrder()
    {
        // Given
        MarkupRenderProcessingService service = CreateService();
        RenderSession session = CreateSession();

        // When
        string result = service.RenderMarkup(
            key: "Default",
            content: "[culturelink[fr-FR]]",
            session: session,
            replacements:
            [
                new MarkupReplacement
                {
                    Old = "?culture=",
                    Value = "ordered"
                }
            ],
            allowContentTags: true);

        // Then
        result.Should()
            .Be(expected: "ordered");
    }

    [Fact]
    public void ShouldRenderRecursiveComponentFragments()
    {
        // Given
        MarkupRenderProcessingService service = CreateService();
        RenderSession session = CreateSession();

        session.ComponentsByName["Parent"] = new PageRenderComponent
        {
            Id = 1,
            Name = "Parent",
            ResourceKey = "Default",
            Content = "[component[Child]]",
            Script = string.Empty
        };

        session.ComponentsByName["Child"] = new PageRenderComponent
        {
            Id = 2,
            Name = "Child",
            ResourceKey = "Default",
            Content = "Nested content",
            Script = string.Empty
        };

        // When
        string result = service.RenderMarkup(
            key: "Default",
            content: "[component[Parent]]",
            session: session,
            replacements: [],
            allowContentTags: true);

        // Then
        result.Should()
            .Contain(expected: "name='Parent'");

        result.Should()
            .Contain(expected: "name='Child'");

        result.Should()
            .Contain(expected: "Nested content");

        result.Should()
            .NotContain(unexpected: "[[render-fragment:");
    }

    [Fact]
    public void ShouldReportAReplacementCycle()
    {
        // Given
        Mock<IWorkflowExecutionBroker> workflowExecutionBroker = new();

        workflowExecutionBroker
            .SetupSequence(expression: broker => broker.Execute(
                baseAddress: It.IsAny<string>(),
                content: It.IsAny<string>()))
            .Returns(value: "A")
            .Returns(value: "B")
            .Returns(value: "A");

        MarkupRenderProcessingService service = CreateService(
            workflowExecutionBroker: workflowExecutionBroker.Object);

        RenderSession session = CreateSession();

        // When
        Action action = () => service.RenderMarkup(
            key: "Default",
            content: "[execute]cycle[/execute]",
            session: session,
            replacements:
            [
                new MarkupReplacement { Old = "A", Value = "[execute]cycle[/execute]" },
                new MarkupReplacement { Old = "B", Value = "[execute]cycle[/execute]" },
                new MarkupReplacement { Old = "[model]", Value = "{}" },
                new MarkupReplacement { Old = "[api[workflow]]", Value = "https://workflow.test" }
            ],
            allowContentTags: true);

        // Then
        action.Should()
            .Throw<InvalidOperationException>()
            .WithMessage(expectedWildcardPattern: "*replacement cycle*");
    }

    [Fact]
    public void ShouldReportWhenReplacementPassLimitIsExceeded()
    {
        // Given
        MarkupRenderProcessingService service = CreateService();
        RenderSession session = CreateSession();

        // When
        Action action = () => service.RenderMarkup(
            key: "Default",
            content: "[grow]",
            session: session,
            replacements:
            [
                new MarkupReplacement
                {
                    Old = "[grow]",
                    Value = "[grow]x"
                }
            ],
            allowContentTags: true);

        // Then
        action.Should()
            .Throw<InvalidOperationException>()
            .WithMessage(expectedWildcardPattern: "*maximum replacement passes*");
    }

    private static MarkupRenderProcessingService CreateService(
        IWorkflowExecutionBroker workflowExecutionBroker = null)
    {
        RegularExpressionBroker regularExpressionBroker = new();

        return new MarkupRenderProcessingService(
            markupRenderService: new MarkupRenderService(
                contentRenderBroker: new TestContentRenderBroker(),
                workflowExecutionBroker:
                    workflowExecutionBroker ?? Mock.Of<IWorkflowExecutionBroker>(),
                cacheBroker: Mock.Of<cCoder.ContentManagement.Brokers.Caching.ICacheBroker>(),
                jsonBroker: new JsonBroker(),
                regularExpressionBroker: regularExpressionBroker,
                renderingUtilityBroker:
                    new cCoder.ContentManagement.Brokers.Rendering.RenderingUtilityBroker()));
    }

    private static RenderSession CreateSession() =>
        new()
        {
            Request = new RenderRequest(),
            Target = new RenderTarget
            {
                Scope = RenderScope.Component,
                ResourceKey = "Default"
            },
            ComponentsByName = new Dictionary<string, PageRenderComponent>(
                comparer: StringComparer.OrdinalIgnoreCase),
            CommonComponentsByName = new Dictionary<string, PageRenderComponent>(
                comparer: StringComparer.OrdinalIgnoreCase),
            ScriptsByName = new Dictionary<string, PageRenderScript>(
                comparer: StringComparer.OrdinalIgnoreCase),
            CommonScriptsByName = new Dictionary<string, PageRenderScript>(
                comparer: StringComparer.OrdinalIgnoreCase),
            EmittedScriptNames = new HashSet<string>(
                comparer: StringComparer.OrdinalIgnoreCase)
        };
}