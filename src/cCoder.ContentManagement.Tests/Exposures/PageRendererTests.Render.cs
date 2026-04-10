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
    public void ShouldDelegateRenderToCoordinationService()
    {
        // Given
        var app = CreateApp();
        PageRenderRequest request = new()
        {
            Host = app.Domain,
            Path = "Summary",
            Theme = string.Empty,
            Culture = string.Empty,
            Edit = true,
        };

        PageRenderResponse expectedResponse = new()
        {
            App = app,
            Page = CreateRenderResult("Rendered Body"),
            Theme = app.DefaultTheme,
            Culture = app.DefaultCultureId,
            Edit = true
        };

        pageRenderCoordinationServiceMock
            .Setup(x => x.Render(request))
            .Returns(expectedResponse);

        // When
        PageRenderResponse response = pageRenderer.Render(request);

        // Then
        response.Should().BeSameAs(expectedResponse);
        response.Edit.Should().BeTrue();
        pageRenderCoordinationServiceMock.Verify(x => x.Render(request), Times.Once);
    }

    [Fact]
    public void ShouldDelegateRenderErrorHandlingToCoordinationService()
    {
        // Given
        var app = CreateApp();
        PageRenderRequest request = new()
        {
            Host = app.Domain,
            Path = "Summary",
            RequestUrl = "https://demo.local/Summary",
        };

        PageRenderResponse expectedResponse = new()
        {
            App = app,
            Page = CreateRenderResult("Boom|trace|https://demo.local/Summary"),
            Theme = app.DefaultTheme,
            Culture = app.DefaultCultureId,
            Edit = false
        };

        pageRenderCoordinationServiceMock
            .Setup(x => x.Render(request))
            .Returns(expectedResponse);

        // When
        PageRenderResponse response = pageRenderer.Render(request);

        // Then
        response.Should().BeSameAs(expectedResponse);
        pageRenderCoordinationServiceMock.Verify(x => x.Render(request), Times.Once);
    }

    [Fact]
    public void ShouldThrowValidationExceptionWhenRequestIsNullForRender()
    {
        // Given
        PageRenderRequest request = null!;

        // When
        Action act = () => pageRenderer.Render(request);

        // Then
        act.Should().Throw<MockException>();
    }
}






