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
using cCoder.ContentManagement.Services.Processings;
using FluentAssertions;
using Xunit;
using RenderApp = cCoder.Data.Models.CMS.App;
using RenderScript = cCoder.Data.Models.CMS.Script;
using RenderTemplate = cCoder.Data.Models.CMS.Template;
using RenderTemplateParams = cCoder.ContentManagement.Models.TemplateRenderParams;
using RenderUser = cCoder.Data.Models.Security.User;
using cCoder.ContentManagement.Tests.Processings;

namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class TemplateRenderProcessingServiceTests
{
    [Fact]
    public async Task ShouldRenderDeclaredSupportedTagTypesForTemplateRoot()
    {
        TemplateRenderProcessingService sut = CreateSut();
        (RenderApp app, RenderUser user, RenderTemplate template) = CreateTemplateRenderContext();
        RenderTemplateParams renderParams = new(app, user, "en-GB");

        metadataCacheMock.Setup(x => x.Get("site-description", "en-GB")).Returns("Meta Description");
        commonObjectCacheMock
            .Setup(x => x.Get<RenderScript>("script|bootstrap"))
            .Returns(new RenderScript { Name = "Bootstrap", Content = "cached-bootstrap" });

        string result = await RenderTestWorkflowServer.RunAsync(workflowBaseUrl =>
            sut.RenderTemplate(template, new { Name = "Taylor" }, renderParams, CreateConfig(workflowBaseUrl)));

        result.Should().Contain("App|Blue|Taylor|bootstrap-script|");
        result.Should().Contain("Hero Taylor");
        result.Should().Contain("<script type='text/javascript'></script>");
        result.Should().NotContain("defer async");
        result.Should().Contain("Meta Description");
        result.Should().Contain("Hello");
        result.Should().Contain("executed");
    }

    [Fact]
    public void ShouldThrowValidationExceptionWhenTemplateIsNull()
    {
        TemplateRenderProcessingService sut = CreateSut();
        (RenderApp app, RenderUser user, _) = CreateTemplateRenderContext();

        Action act = () => sut.RenderTemplate(null!, new { Name = "Taylor" }, new RenderTemplateParams(app, user, "en-GB"), CreateConfig("http://127.0.0.1/"));

        act.Should().Throw<ValidationException>();
    }
}







