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
using System.Security;
using cCoder.ContentManagement.Exposures;



using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Coordinations;

public partial class PageRenderCoordinationServiceTests
{
    [Fact]
    public void ShouldRenderPageUsingResolvedDefaults()
    {
        App app = CreateApp();
        PageRenderRequest request = new()
        {
            Host = app.Domain,
            Path = "Summary",
            Theme = string.Empty,
            Culture = string.Empty,
            Edit = true
        };

        RenderResult renderResult = CreateRenderResult("Rendered Body");

        appOrchestrationServiceMock.Setup(x => x.GetByDomain(app.Domain, true)).Returns(app);
        appOrchestrationServiceMock.Setup(x => x.GetAll(false)).Returns(new[] { app }.AsQueryable());
        layoutOrchestrationServiceMock.Setup(x => x.GetAll(false)).Returns(app.Layouts.AsQueryable());
        pageOrchestrationServiceMock.Setup(x => x.GetAll(true)).Returns(new[]
        {
            new Page
            {
                Id = 10,
                AppId = app.Id,
                Name = "Summary",
                Path = "Summary",
                App = app,
                PageInfo = [],
                Contents = [new Content { Name = "Body", Html = "Rendered Body" }],
                Roles = []
            }
        }.AsQueryable());
        pageOrchestrationServiceMock.Setup(x => x.GetAll(false)).Returns(new[]
        {
            new Page
            {
                Id = 10,
                AppId = app.Id,
                Name = "Summary",
                Path = "Summary",
                App = app,
                PageInfo = [],
                Contents = [new Content { Name = "Body", Html = "Rendered Body" }],
                Roles = []
            }
        }.AsQueryable());

        pageRenderOrchestrationServiceMock
            .Setup(x => x.Render(It.IsAny<Page>(), It.IsAny<User>(), app.DefaultTheme, app.DefaultCultureId, false))
            .Returns(renderResult);

        PageRenderResponse response = coordinationService.Render(request);

        response.App.Id.Should().Be(app.Id);
        response.Page.Should().BeSameAs(renderResult);
        response.Theme.Should().Be(app.DefaultTheme);
        response.Culture.Should().Be(app.DefaultCultureId);
        response.Edit.Should().BeTrue();
        componentOrchestrationServiceMock.Verify(x => x.GetAll(false), Times.AtLeastOnce);
        scriptOrchestrationServiceMock.Verify(x => x.GetAll(false), Times.AtLeastOnce);
        resourceOrchestrationServiceMock.Verify(x => x.GetAll(false), Times.AtLeastOnce);
        pageOrchestrationServiceMock.Verify(x => x.GetAll(true), Times.AtLeastOnce);
    }

    [Fact]
    public void ShouldFallbackToErrorRenderWhenPrimaryRenderThrows()
    {
        App app = CreateApp();
        PageRenderRequest request = new()
        {
            Host = app.Domain,
            Path = "Summary",
            RequestUrl = "https://demo.local/Summary"
        };

        appOrchestrationServiceMock.Setup(x => x.GetByDomain(app.Domain, true)).Returns(app);
        appOrchestrationServiceMock.Setup(x => x.GetAll(false)).Returns(new[] { app }.AsQueryable());
        layoutOrchestrationServiceMock.Setup(x => x.GetAll(false)).Returns(app.Layouts.AsQueryable());
        pageOrchestrationServiceMock.Setup(x => x.GetAll(true)).Returns(new[]
        {
            new Page
            {
                Id = 10,
                AppId = app.Id,
                Name = "Summary",
                Path = "Summary",
                App = app,
                PageInfo = [],
                Contents = [new Content { Name = "Body", Html = "Rendered Body" }],
                Roles = []
            },
            new Page
            {
                Id = 11,
                AppId = app.Id,
                Name = "Error",
                Path = "Error",
                App = app,
                PageInfo = [],
                Contents = [new Content { Name = "Body", Html = "[problem[message]]|[problem[detail]]|[problem[url]]" }],
                Roles = []
            }
        }.AsQueryable());
        pageOrchestrationServiceMock.Setup(x => x.GetAll(false)).Returns(new[]
        {
            new Page
            {
                Id = 10,
                AppId = app.Id,
                Name = "Summary",
                Path = "Summary",
                App = app,
                PageInfo = [],
                Contents = [new Content { Name = "Body", Html = "Rendered Body" }],
                Roles = []
            },
            new Page
            {
                Id = 11,
                AppId = app.Id,
                Name = "Error",
                Path = "Error",
                App = app,
                PageInfo = [],
                Contents = [new Content { Name = "Body", Html = "[problem[message]]|[problem[detail]]|[problem[url]]" }],
                Roles = []
            }
        }.AsQueryable());

        pageRenderOrchestrationServiceMock
            .SetupSequence(x => x.Render(It.IsAny<Page>(), It.IsAny<User>(), app.DefaultTheme, app.DefaultCultureId, It.IsAny<bool>()))
            .Throws(new InvalidOperationException("Boom"))
            .Returns(CreateRenderResult("[problem[message]]|[problem[detail]]|[problem[url]]"));

        PageRenderResponse response = coordinationService.Render(request);

        response.Page.BodyHtml.Should().Contain("Boom");
        response.Page.BodyHtml.Should().Contain("https://demo.local/Summary");
    }

    [Fact]
    public void ShouldThrowValidationExceptionWhenRequestIsNull()
    {
        PageRenderRequest request = null!;

        Action act = () => coordinationService.Render(request);

        act.Should().Throw<ValidationException>().WithMessage("request is required.");
    }

    [Fact]
    public void ShouldThrowValidationExceptionWhenExceptionIsMissingForRenderError()
    {
        PageRenderRequest request = new()
        {
            Host = "demo.local",
            Exception = null
        };

        Action act = () => coordinationService.RenderError(request);

        act.Should().Throw<ValidationException>().WithMessage("Exception is required.");
    }

    [Fact]
    public void ShouldReturnNotFoundRenderResultWhenPageDoesNotExist()
    {
        App app = CreateApp();
        currentUser = TestUsers.WithPrivilege("app_admin", app.Id);

        appOrchestrationServiceMock.Setup(x => x.GetAll(false)).Returns(new[] { app }.AsQueryable());
        layoutOrchestrationServiceMock.Setup(x => x.GetAll(false)).Returns(app.Layouts.AsQueryable());
        pageOrchestrationServiceMock.Setup(x => x.GetAll(true)).Returns(Array.Empty<Page>().AsQueryable());
        pageOrchestrationServiceMock.Setup(x => x.GetAll(false)).Returns(Array.Empty<Page>().AsQueryable());
        pageRenderOrchestrationServiceMock
            .Setup(x => x.Render(It.IsAny<Page>(), It.IsAny<User>(), "Default", string.Empty, false))
            .Returns(CreateRenderResult());

        RenderResult result = coordinationService.Render(app.Id, "missing", "Default", string.Empty);

        result.StatusCode.Should().Be(404);
        pageRenderOrchestrationServiceMock.Verify(
            x => x.Render(It.Is<Page>(page => page.Path == "missing"), It.IsAny<User>(), "Default", string.Empty, false),
            Times.Once);
    }

    [Fact]
    public void ShouldRenderLoginContentWhenUserCannotReadPage()
    {
        App app = CreateApp();
        currentUser = TestUsers.WithoutPrivileges();
        authorizationBrokerMock.Setup(x => x.IsAdminOfApp(app.Id)).Returns(false);

        Role pageRole = new()
        {
            Id = Guid.NewGuid(),
            AppId = app.Id,
            Name = "Members",
            Privileges = ["page_read"]
        };

        Page protectedPage = new()
        {
            Id = 10,
            AppId = app.Id,
            Name = "Home",
            Path = string.Empty,
            App = app,
            PageInfo = [],
            Contents = [new Content { Name = "Body", Html = "Hello world" }],
            Roles = [new PageRole { RoleId = pageRole.Id, Role = pageRole }]
        };

        appOrchestrationServiceMock.Setup(x => x.GetAll(false)).Returns(new[] { app }.AsQueryable());
        layoutOrchestrationServiceMock.Setup(x => x.GetAll(false)).Returns(app.Layouts.AsQueryable());
        pageOrchestrationServiceMock.Setup(x => x.GetAll(true)).Returns(new[] { protectedPage }.AsQueryable());
        pageOrchestrationServiceMock.Setup(x => x.GetAll(false)).Returns(new[] { protectedPage }.AsQueryable());
        pageRenderOrchestrationServiceMock
            .Setup(x => x.Render(It.IsAny<Page>(), It.IsAny<User>(), "Default", string.Empty, false))
            .Returns(CreateRenderResult());

        coordinationService.Render(app.Id, string.Empty, "Default", string.Empty);

        pageRenderOrchestrationServiceMock.Verify(
            x => x.Render(
                It.Is<Page>(page => page.Contents.Any(content => content.Html == "[component[login]]")),
                It.IsAny<User>(),
                "Default",
                string.Empty,
                false),
            Times.Once);
    }

    [Fact]
    public void ShouldHydratePageCollectionsBeforeRendering()
    {
        App app = CreateApp();
        currentUser = TestUsers.WithPrivilege("app_admin", app.Id);

        Page page = new()
        {
            Id = 21,
            AppId = app.Id,
            Name = "Admin",
            Path = "Admin",
            App = app
        };

        PageInfo[] pageInfos =
        [
            new()
            {
                Id = 1,
                PageId = page.Id,
                CultureId = string.Empty,
                Title = "Admin"
            }
        ];

        Content[] contents =
        [
            new()
            {
                Id = 1,
                PageId = page.Id,
                Name = "body",
                Html = "[component[DetailedNav]]"
            }
        ];

        PageRole[] roles =
        [
            new()
            {
                PageId = page.Id,
                RoleId = Guid.NewGuid()
            }
        ];

        appOrchestrationServiceMock.Setup(x => x.GetAll(false)).Returns(new[] { app }.AsQueryable());
        layoutOrchestrationServiceMock.Setup(x => x.GetAll(false)).Returns(app.Layouts.AsQueryable());
        pageOrchestrationServiceMock.Setup(x => x.GetAll(true)).Returns(new[] { page }.AsQueryable());
        pageOrchestrationServiceMock.Setup(x => x.GetAll(false)).Returns(new[] { page }.AsQueryable());
        pageInfoOrchestrationServiceMock.Setup(x => x.GetAll(true)).Returns(pageInfos.AsQueryable());
        contentOrchestrationServiceMock.Setup(x => x.GetAll(true)).Returns(contents.AsQueryable());
        pageRoleOrchestrationServiceMock.Setup(x => x.GetAll(true)).Returns(roles.AsQueryable());
        pageRenderOrchestrationServiceMock
            .Setup(x => x.Render(It.IsAny<Page>(), It.IsAny<User>(), "Default", string.Empty, false))
            .Returns(CreateRenderResult());

        coordinationService.Render(app.Id, "Admin", "Default", string.Empty);

        pageRenderOrchestrationServiceMock.Verify(
            x => x.Render(
                It.Is<Page>(renderPage =>
                    renderPage.PageInfo.SequenceEqual(pageInfos)
                    && renderPage.Contents.SequenceEqual(contents)
                    && renderPage.Roles.SequenceEqual(roles)),
                It.IsAny<User>(),
                "Default",
                string.Empty,
                false),
            Times.Once);
    }

    [Fact]
    public void ShouldUseBodySlotForMissingPageAndGatedPageFallbacks()
    {
        App app = CreateApp();
        currentUser = TestUsers.WithoutPrivileges();
        authorizationBrokerMock.Setup(x => x.IsAdminOfApp(app.Id)).Returns(false);

        Role pageRole = new()
        {
            Id = Guid.NewGuid(),
            AppId = app.Id,
            Name = "Members",
            Privileges = ["page_read"]
        };

        Page protectedPage = new()
        {
            Id = 10,
            AppId = app.Id,
            Name = "Admin",
            Path = "Admin",
            App = app,
            Roles = [new PageRole { RoleId = pageRole.Id, Role = pageRole }]
        };

        appOrchestrationServiceMock.Setup(x => x.GetAll(false)).Returns(new[] { app }.AsQueryable());
        layoutOrchestrationServiceMock.Setup(x => x.GetAll(false)).Returns(app.Layouts.AsQueryable());
        pageOrchestrationServiceMock.SetupSequence(x => x.GetAll(true))
            .Returns(Array.Empty<Page>().AsQueryable())
            .Returns(new[] { protectedPage }.AsQueryable());
        pageOrchestrationServiceMock.Setup(x => x.GetAll(false)).Returns(new[] { protectedPage }.AsQueryable());
        pageRenderOrchestrationServiceMock
            .Setup(x => x.Render(It.IsAny<Page>(), It.IsAny<User>(), "Default", string.Empty, false))
            .Returns(CreateRenderResult());

        coordinationService.Render(app.Id, "missing", "Default", string.Empty);
        coordinationService.Render(app.Id, "Admin", "Default", string.Empty);

        pageRenderOrchestrationServiceMock.Verify(
            x => x.Render(
                It.Is<Page>(renderPage => renderPage.Path == "missing"
                    && renderPage.Contents.Any(content => content.Name == "body" && content.Html == "[component[NotFound]]")),
                It.IsAny<User>(),
                "Default",
                string.Empty,
                false),
            Times.Once);

        pageRenderOrchestrationServiceMock.Verify(
            x => x.Render(
                It.Is<Page>(renderPage =>
                    renderPage.Path == "Admin"
                    && renderPage.Contents.Any(content => content.Name == "body" && content.Html == "[component[login]]")),
                It.IsAny<User>(),
                "Default",
                string.Empty,
                false),
            Times.Once);
    }

    [Fact]
    public void ShouldThrowSecurityExceptionWhenAppIsUnknown()
    {
        appOrchestrationServiceMock.Setup(x => x.GetAll(false)).Returns(Array.Empty<App>().AsQueryable());

        Action act = () => coordinationService.Render(1, string.Empty, "Default", string.Empty);

        act.Should().Throw<SecurityException>().WithMessage("Unknown Domain!");
    }
}




