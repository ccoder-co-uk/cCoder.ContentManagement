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
using Moq;
using Xunit;
using RenderApp = cCoder.Data.Models.CMS.App;
using RenderPage = cCoder.Data.Models.CMS.Page;
using RenderScript = cCoder.Data.Models.CMS.Script;
using RenderUser = cCoder.Data.Models.Security.User;
using cCoder.ContentManagement.Tests.Processings;

namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PageRenderProcessingServiceTests
{
    [Fact]
    public async Task ShouldRenderDeclaredSupportedTagTypesForPageRoot()
    {
        PageRenderProcessingService sut = CreateSut();
        RenderApp app = CreateApp();
        RenderPage page = app.Pages.First(foundPage => foundPage.Id == 10);
        RenderUser user = CreateUser();

        metadataReaderBroker.Set("site-description", "en-GB", "Meta Description");

        RenderResult result = await RenderTestWorkflowServer.RunAsync(workflowBaseUrl =>
            sut.RenderPage(page, user, CreateConfig(workflowBaseUrl), "Default", "en-GB"));

        result.HeaderHtml.Should().Contain("<title>Home</title>");
        result.HeaderHtml.Should().Contain("Meta Description");
        result.HeaderHtml.Should().Contain("bootstrap-script");
        result.BodyHtml.Should().Contain("Body Content");
        result.BodyHtml.Should().Contain("Hero Component");
        result.BodyHtml.Should().Contain("<script type='text/javascript'>hero-component-script</script>");
        result.BodyHtml.Should().Contain("Hello|Hi|Greeting Description");
        result.BodyHtml.Should().Contain("Blue|App|");
        result.BodyHtml.Should().Contain("executed");
        result.BodyHtml.Should().Contain("href='/Summary'");
        result.BodyHtml.Should().Contain("dropdown-menu");

        metadataReaderBroker.Requests.Should().ContainSingle(request =>
            request.Name == "site-description" && request.Culture == "en-GB");
    }

    [Fact]
    public void ShouldThrowValidationExceptionWhenPageIsNull()
    {
        PageRenderProcessingService sut = CreateSut();

        Action act = () => sut.RenderPage(null!, CreateUser(), CreateConfig("http://127.0.0.1/"), "Default", "en-GB");

        act.Should().Throw<ValidationException>();
    }
}







