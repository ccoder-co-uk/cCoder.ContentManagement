// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.PageRendering;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.ContentManagement.Rendering.Brokers;
using cCoder.ContentManagement.Rendering.Services.Foundations;
using cCoder.ContentManagement.Rendering.Services.Processings;
using FluentAssertions;
using Moq;
using Xunit;
using cCoder.ContentManagement.Tests.Brokers.Rendering;

namespace cCoder.ContentManagement.Tests.Processings;

public sealed partial class ScriptTagHandlingProcessingServiceTests
{
    [Fact]
    public void ShouldEmitDuplicateAndNestedScriptRequestsOnlyOnce()
    {
        // Given
        RenderSession session = new()
        {
            Request = new RenderRequest { AppId = 7 },
            Target = new RenderTarget
            {
                Scope = RenderScope.Component,
                ResourceKey = "Default",
                HeaderMarkup = string.Empty,
                BodyMarkup = "[script[Widgets.Dialog]][script[Widgets.Dialog]]",
                AllowBodyContentTags = true
            },
            ScriptsByName = new Dictionary<string, PageRenderScript>(StringComparer.OrdinalIgnoreCase)
            {
                ["Widgets.Dialog"] = new PageRenderScript
                {
                    Name = "Widgets.Dialog",
                    Content = "class Dialog { }[script[Widgets.Helper]]"
                },
                ["Widgets.Helper"] = new PageRenderScript
                {
                    Name = "Widgets.Helper",
                    Content = "class Helper { }"
                }
            },
            CommonScriptsByName = new Dictionary<string, PageRenderScript>(),
            EmittedScriptNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        };

        MarkupRenderProcessingService service = new(
            markupRenderService: new MarkupRenderService(
                contentRenderBroker: new TestContentRenderBroker(),
                workflowExecutionBroker: Mock.Of<IWorkflowExecutionBroker>(),
                cacheBroker: Mock.Of<cCoder.ContentManagement.Brokers.Caching.ICacheBroker>(),
                jsonBroker: Mock.Of<IJsonBroker>(),
                regularExpressionBroker: new RegularExpressionBroker(),
                renderingUtilityBroker:
                    new cCoder.ContentManagement.Brokers.Rendering.RenderingUtilityBroker()));

        // When
        RenderSession nestedSession = service.RenderRenderSession(
            renderSession: session);

        string nestedResult = nestedSession.Output.BodyMarkup;

        session.Target.BodyMarkup =
            "[script[Widgets.Dialog]][script[Widgets.Helper]]";

        string repeatedResult = service.RenderRenderSession(
            renderSession: session)
            .Output.BodyMarkup;

        // Then
        nestedResult.Should()
            .Be(expected: "class Dialog { }class Helper { }");

        repeatedResult.Should()
            .BeEmpty();
    }
}