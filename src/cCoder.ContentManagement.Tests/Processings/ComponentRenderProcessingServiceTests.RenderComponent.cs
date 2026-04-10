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
using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using Xunit;
using RenderApp = cCoder.Data.Models.CMS.App;
using RenderComponent = cCoder.Data.Models.CMS.Component;
using RenderComponentParams = cCoder.ContentManagement.Models.ComponentRenderParams;
using RenderScript = cCoder.Data.Models.CMS.Script;
using RenderUser = cCoder.Data.Models.Security.User;
using cCoder.ContentManagement.Tests.Processings;

namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class ComponentRenderProcessingServiceTests
{
    [Fact]
    public async Task ShouldRenderDeclaredSupportedTagTypesForComponentRoot()
    {
        (RenderApp app, RenderUser user, RenderComponent component, RenderComponentParams renderParams) =
            CreateComponentRenderContext();

        metadataCacheMock.Setup(x => x.Get("site-description", "en-GB")).Returns("Meta Description");
        commonObjectCacheMock
            .Setup(x => x.Get<RenderScript>("script|bootstrap"))
            .Returns(new RenderScript { Name = "Bootstrap", Content = "cached-bootstrap" });
        renderFileContentServiceMock.Setup(x => x.GetLatestTextContent(app.Id, "snippets/info")).Returns("snippet-text");

        string result = await RenderTestWorkflowServer.RunAsync(
            workflowBaseUrl => CreateSut(workflowBaseUrl).RenderComponent(component, renderParams));

        result.Should().Contain("snippet-text");
        result.Should().Contain("bootstrap-script");
        result.Should().Contain("Child Component");
        result.Should().Contain("<script type='text/javascript'></script>");
        result.Should().NotContain("defer async");
        result.Should().Contain("Meta Description");
        result.Should().Contain("Hello");
        result.Should().Contain("Blue");
        result.Should().Contain("executed");
    }

    [Fact]
    public void ShouldThrowValidationExceptionWhenComponentIsNull()
    {
        (_, _, _, RenderComponentParams renderParams) = CreateComponentRenderContext();

        Action act = () => CreateSut("http://127.0.0.1/").RenderComponent(null!, renderParams);

        act.Should().Throw<ValidationException>();
    }
}








