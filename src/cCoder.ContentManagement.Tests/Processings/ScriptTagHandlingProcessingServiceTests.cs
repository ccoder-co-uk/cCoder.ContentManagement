// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.PageRendering;
using cCoder.ContentManagement.Rendering.Brokers;
using cCoder.ContentManagement.Services.Processings.PageRendering;
using FluentAssertions;
using Moq;
using Xunit;

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

        ScriptTagHandlingProcessingService service = new(
            scriptReaderBroker: Mock.Of<IScriptReaderBroker>());

        TagHandlingOperation operation = new()
        {
            Session = session,
            Content = "[script[Widgets.Dialog]][script[Widgets.Dialog]]"
        };

        // When
        TagHandlingOperation duplicateResult = service.HandleTagHandlingOperation(operation: operation);
        TagHandlingOperation nestedResult = service.HandleTagHandlingOperation(operation: duplicateResult);

        TagHandlingOperation repeatedResult = service.HandleTagHandlingOperation(
            operation: new TagHandlingOperation
            {
                Session = session,
                Content = "[script[Widgets.Dialog]][script[Widgets.Helper]]"
            });

        // Then
        nestedResult.Content.Should()
            .Be(expected: "class Dialog { }class Helper { }");

        repeatedResult.Content.Should()
            .BeEmpty();
    }
}