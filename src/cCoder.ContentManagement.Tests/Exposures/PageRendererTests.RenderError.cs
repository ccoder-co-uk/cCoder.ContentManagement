// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.Security;
using ComponentRenderParams = cCoder.ContentManagement.Models.ComponentRenderParams;
using Config = cCoder.ContentManagement.Models.Config;
using PageRenderParams = cCoder.ContentManagement.Models.PageRenderParams;
using PageRoleInfo = cCoder.ContentManagement.Models.PageRoleInfo;
using RenderParams = cCoder.ContentManagement.Models.RenderParams;
using RenderResult = cCoder.ContentManagement.Models.RenderResult;
using TemplateRenderParams = cCoder.ContentManagement.Models.TemplateRenderParams;
using cCoder.ContentManagement.Exposures;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.CMS.Exposures;

public partial class PageRendererTests
{
    [Fact]
    public void ShouldDelegateRenderErrorToCoordinationService()
    {
        // Given
        var app = CreateApp();

        PageRenderRequest request = new()
        {
            Host = app.Domain,
            Theme = "Custom",
            Culture = "fr-FR",
            RequestUrl = "https://demo.local/Summary",
            Exception = new InvalidOperationException(message: "Problem happened"),
        };

        PageRenderResponse expectedResponse = new()
        {
            App = app,
            Page = CreateRenderResult(bodyHtml: "Problem happened|trace|https://demo.local/Summary"),
            Theme = "Custom",
            Culture = "fr-FR",
            Edit = false
        };

        pageRenderCoordinationServiceMock
            .Setup(expression: x => x.RenderError(request: request))
            .Returns(value: expectedResponse);

        // When
        PageRenderResponse response = pageRenderer.RenderError(request: request);

        // Then

        response.Should()
            .BeSameAs(expected: expectedResponse);

        pageRenderCoordinationServiceMock.Verify(expression: x => x.RenderError(request: request), times: Times.Once);
    }

    [Fact]
    public void ShouldThrowWhenRenderErrorRequestIsNotConfigured()
    {
        // Given
        PageRenderRequest request = new()
        {
            Host = "demo.local",
            Exception = null,
        };

        // When
        Action act = () => pageRenderer.RenderError(request: request);

        // Then

        act.Should()
            .Throw<MockException>();
    }
}