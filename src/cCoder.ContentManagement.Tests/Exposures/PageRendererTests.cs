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



using cCoder.ContentManagement.Services.Coordinations;
using Moq;

namespace cCoder.ContentManagement.Tests.CMS.Exposures;

public partial class PageRendererTests
{
    private readonly Mock<IPageRenderCoordinationService> pageRenderCoordinationServiceMock = new(MockBehavior.Strict);
    private readonly PageRenderer pageRenderer;

    public PageRendererTests()
    {
        pageRenderer = new PageRenderer(pageRenderCoordinationServiceMock.Object);
    }

    private static App CreateApp() =>
        new()
        {
            Id = 1,
            Name = "Demo",
            Domain = "demo.local",
            DefaultTheme = "Ocean",
            DefaultCultureId = "en-GB",
            ConfigJson = "{}",
            Pages = [],
            Components = [],
            Scripts = [],
            Templates = [],
            Resources = [],
            Layouts = []
        };

    private static RenderResult CreateRenderResult(string bodyHtml = "Body") =>
        new()
        {
            HeaderHtml = "Header",
            BodyHtml = bodyHtml,
            Theme = "Ocean",
            Culture = "en-GB",
            Edit = false
        };
}




