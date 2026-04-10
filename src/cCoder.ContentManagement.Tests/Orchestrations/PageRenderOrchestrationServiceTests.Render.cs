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
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Services.Processings;
using FluentAssertions;
using Moq;
using Xunit;
using cCoder.Core.Services.Tests;

namespace cCoder.ContentManagement.Tests.Orchestrations;

public partial class PageRenderOrchestrationServiceTests
{
    [Fact]
    public void ShouldDelegatePreparedPageToRenderProcessingService()
    {
        Page page = new()
        {
            Id = 10,
            AppId = 1,
            Name = "Home",
            Path = string.Empty,
            App = new App
            {
                Id = 1,
                Name = "App",
                Domain = "app.local",
                DefaultCultureId = string.Empty,
                DefaultTheme = "Default",
                ConfigJson = "{}",
                Layouts = [],
                Pages = [],
                Components = [],
                Resources = [],
                Scripts = [],
                Templates = []
            },
            PageInfo = [],
            Contents = [],
            Roles = []
        };

        User user = TestUsers.WithPrivilege("app_admin", 1);
        RenderResult expected = new() { StatusCode = 200 };
        Mock<IPageRenderProcessingService> processingServiceMock = new();
        PageRenderOrchestrationService orchestrationService = new(new Config(), processingServiceMock.Object);

        processingServiceMock
            .Setup(x => x.RenderPage(page, user, It.IsAny<Config>(), "Default", string.Empty, true))
            .Returns(expected);

        RenderResult actual = orchestrationService.Render(page, user, "Default", string.Empty, true);

        actual.Should().BeSameAs(expected);
        processingServiceMock.VerifyAll();
    }
}







