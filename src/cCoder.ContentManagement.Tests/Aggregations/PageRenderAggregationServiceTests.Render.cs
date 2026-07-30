// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.Security;
using ComponentRenderParams = cCoder.ContentManagement.Models.ComponentRenderParams;
using Config = cCoder.ContentManagement.Models.ContentManagementConfiguration;
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

namespace cCoder.Core.Services.Tests.CMS.Aggregations;

public partial class PageRenderAggregationServiceTests
{
    [Fact]
    public void ShouldRenderPageUsingResolvedDefaults()
    {
        // Given
        App app = CreateApp();

        PageRenderRequest request = new()
        {
            Host = app.Domain,
            Path = "Summary",
            Theme = string.Empty,
            Culture = string.Empty,
            Edit = true
        };

        RenderResult renderResult = CreateRenderResult(bodyHtml: "Rendered Body");

        appOrchestrationServiceMock.Setup(expression: x => x.GetByDomainApp(domain: app.Domain, ignoreFilters: true))
            .Returns(value: app);

        appOrchestrationServiceMock.Setup(expression: x => x.GetAllApp())
            .Returns(value: new[] { app }.AsQueryable());

        layoutOrchestrationServiceMock.Setup(expression: x => x.GetAllLayout())
            .Returns(value: app.Layouts.AsQueryable());

        pageOrchestrationServiceMock.Setup(expression: x => x.GetAllPage(ignoreFilters: true))
            .Returns(value: new[]
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

        pageOrchestrationServiceMock.Setup(expression: x => x.GetAllPage())
            .Returns(value: new[]
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

        SetupRenderResult(renderResult: renderResult);

        // When
        PageRenderResponse response =
            aggregationService.RenderPageRenderRequestPageRenderResponse(request: request);

        // Then
        response.App.Id.Should()
            .Be(expected: app.Id);

        response.Page.Should()
            .BeSameAs(expected: renderResult);

        response.Theme.Should()
            .Be(expected: app.DefaultTheme);

        response.Culture.Should()
            .Be(expected: app.DefaultCultureId);

        response.Edit.Should()
            .BeTrue();

        componentOrchestrationServiceMock.Verify(expression: x => x.GetAllComponent(), times: Times.AtLeastOnce);
        scriptOrchestrationServiceMock.Verify(expression: x => x.GetAllScript(), times: Times.AtLeastOnce);
        resourceOrchestrationServiceMock.Verify(expression: x => x.GetAllResource(), times: Times.AtLeastOnce);
        pageOrchestrationServiceMock.Verify(expression: x => x.GetAllPage(ignoreFilters: true), times: Times.AtLeastOnce);
    }

    [Fact]
    public void ShouldFallbackToErrorRenderWhenPrimaryRenderThrows()
    {
        // Given
        App app = CreateApp();

        PageRenderRequest request = new()
        {
            Host = app.Domain,
            Path = "Summary",
            RequestUrl = "https://demo.local/Summary"
        };

        appOrchestrationServiceMock.Setup(expression: x => x.GetByDomainApp(domain: app.Domain, ignoreFilters: true))
            .Returns(value: app);

        appOrchestrationServiceMock.Setup(expression: x => x.GetAllApp())
            .Returns(value: new[] { app }.AsQueryable());

        layoutOrchestrationServiceMock.Setup(expression: x => x.GetAllLayout())
            .Returns(value: app.Layouts.AsQueryable());

        pageOrchestrationServiceMock.Setup(expression: x => x.GetAllPage(ignoreFilters: true))
            .Returns(value: new[]
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

        pageOrchestrationServiceMock.Setup(expression: x => x.GetAllPage())
            .Returns(value: new[]
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
            .SetupSequence(expression: service => service.ProcessPageRenderOperation(
                operation: It.Is<PageRenderOperation>(match: operation =>
                    operation.OperationType != PageRenderOperationType.UserCanPage)))
            .Throws(exception: new InvalidOperationException(message: "Boom"))
            .Returns(value: CreatePageRenderOperation(
                renderResult: CreateRenderResult(
                    bodyHtml: "[problem[message]]|[problem[detail]]|[problem[url]]")));

        // When
        PageRenderResponse response =
            aggregationService.RenderPageRenderRequestPageRenderResponse(request: request);

        // Then
        response.Page.BodyHtml.Should()
            .Contain(expected: "Boom");

        response.Page.BodyHtml.Should()
            .Contain(expected: "https://demo.local/Summary");
    }

    [Fact]
    public void ShouldThrowValidationExceptionWhenRequestIsNull()
    {
        // Given
        PageRenderRequest request = null!;

        // When
        Action act = () =>
            aggregationService.RenderPageRenderRequestPageRenderResponse(request: request);

        // Then
        act.Should()
            .Throw<ValidationException>()
            .WithMessage(expectedWildcardPattern: "request is required.");
    }

    [Fact]
    public void ShouldThrowValidationExceptionWhenExceptionIsMissingForRenderError()
    {
        // Given
        PageRenderRequest request = new()
        {
            Host = "demo.local",
            Exception = null
        };

        // When
        Action act = () =>
            aggregationService.RenderErrorPageRenderRequestPageRenderResponse(request: request);

        // Then
        act.Should()
            .Throw<ValidationException>()
            .WithMessage(expectedWildcardPattern: "Exception is required.");
    }

    [Fact]
    public void ShouldReturnNotFoundRenderResultWhenPageDoesNotExist()
    {
        // Given
        App app = CreateApp();
        currentUser = TestUsers.WithPrivilege(privilege: "app_admin", appId: app.Id);

        appOrchestrationServiceMock.Setup(expression: x => x.GetAllApp())
            .Returns(value: new[] { app }.AsQueryable());

        layoutOrchestrationServiceMock.Setup(expression: x => x.GetAllLayout())
            .Returns(value: app.Layouts.AsQueryable());

        pageOrchestrationServiceMock.Setup(expression: x => x.GetAllPage(ignoreFilters: true))
            .Returns(value: Array.Empty<Page>()
            .AsQueryable());

        pageOrchestrationServiceMock.Setup(expression: x => x.GetAllPage())
            .Returns(value: Array.Empty<Page>()
            .AsQueryable());

        SetupRenderResult(renderResult: CreateRenderResult());

        // When
        RenderResult result = aggregationService.RenderRenderResult(appId: app.Id, path: "missing", theme: "Default", culture: string.Empty);

        // Then
        result.StatusCode.Should()
            .Be(expected: 404);

        pageRenderOrchestrationServiceMock.Verify(
            expression: service => service.ProcessPageRenderOperation(
                operation: It.Is<PageRenderOperation>(match: operation =>
                    operation.SourcePage.Path == "missing")),
            times: Times.Once);
    }

    [Fact]
    public void ShouldRenderLoginContentWhenUserCannotReadPage()
    {
        // Given
        App app = CreateApp();
        currentUser = TestUsers.WithoutPrivileges();

        pageRenderOrchestrationServiceMock.Setup(expression: x => x.IsAdminOfApp(appId: app.Id))
            .Returns(value: false);

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

        appOrchestrationServiceMock.Setup(expression: x => x.GetAllApp())
            .Returns(value: new[] { app }.AsQueryable());

        layoutOrchestrationServiceMock.Setup(expression: x => x.GetAllLayout())
            .Returns(value: app.Layouts.AsQueryable());

        pageOrchestrationServiceMock.Setup(expression: x => x.GetAllPage(ignoreFilters: true))
            .Returns(value: new[] { protectedPage }.AsQueryable());

        pageOrchestrationServiceMock.Setup(expression: x => x.GetAllPage())
            .Returns(value: new[] { protectedPage }.AsQueryable());

        SetupRenderResult(renderResult: CreateRenderResult());

        // When
        aggregationService.RenderRenderResult(appId: app.Id, path: string.Empty, theme: "Default", culture: string.Empty);

        // Then
        pageRenderOrchestrationServiceMock.Verify(
            expression: service => service.ProcessPageRenderOperation(
                operation: It.Is<PageRenderOperation>(match: operation =>
                    operation.SourcePage.Contents.Any(
                        predicate: content => content.Html == "[component[login]]"))),
            times: Times.Once);
    }

    [Fact]
    public void ShouldHydratePageCollectionsBeforeRendering()
    {
        // Given
        App app = CreateApp();
        currentUser = TestUsers.WithPrivilege(privilege: "app_admin", appId: app.Id);

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

        appOrchestrationServiceMock.Setup(expression: x => x.GetAllApp())
            .Returns(value: new[] { app }.AsQueryable());

        layoutOrchestrationServiceMock.Setup(expression: x => x.GetAllLayout())
            .Returns(value: app.Layouts.AsQueryable());

        pageOrchestrationServiceMock.Setup(expression: x => x.GetAllPage(ignoreFilters: true))
            .Returns(value: new[] { page }.AsQueryable());

        pageOrchestrationServiceMock.Setup(expression: x => x.GetAllPage())
            .Returns(value: new[] { page }.AsQueryable());

        pageInfoOrchestrationServiceMock.Setup(expression: x => x.GetAllPageInfo(ignoreFilters: true))
            .Returns(value: pageInfos.AsQueryable());

        contentOrchestrationServiceMock.Setup(expression: x => x.GetAllContent(ignoreFilters: true))
            .Returns(value: contents.AsQueryable());

        pageRoleOrchestrationServiceMock.Setup(expression: x => x.GetAllPageRole(ignoreFilters: true))
            .Returns(value: roles.AsQueryable());

        SetupRenderResult(renderResult: CreateRenderResult());

        // When
        aggregationService.RenderRenderResult(appId: app.Id, path: "Admin", theme: "Default", culture: string.Empty);

        // Then
        pageRenderOrchestrationServiceMock.Verify(
            expression: service => service.ProcessPageRenderOperation(
                operation: It.Is<PageRenderOperation>(match: operation =>
                    operation.OperationType != PageRenderOperationType.UserCanPage
                    && operation.SourcePage.PageInfo.SequenceEqual(second: pageInfos)
                    && operation.SourcePage.Contents.SequenceEqual(second: contents)
                    && operation.SourcePage.Roles.SequenceEqual(second: roles))),
            times: Times.Once);
    }

    [Fact]
    public void ShouldRenderPublicPageWhenHydratedPageRolesContainPrivileges()
    {
        // Given
        App app = CreateApp();

        Role guestRole = new()
        {
            Id = Guid.NewGuid(),
            AppId = app.Id,
            Name = "Guests",
            Privileges = ["page_read"]
        };

        currentUser = new User
        {
            Id = "Guest",
            DefaultCultureId = string.Empty,
            DisplayName = "Guest",
            Email = "guest@example.com",
            Roles =
            [
                new UserRole
                {
                    RoleId = guestRole.Id,
                    Role = guestRole
                }
            ]
        };

        Page page = new()
        {
            Id = 19,
            AppId = app.Id,
            Name = "Home",
            Path = string.Empty,
            App = app,
            PageInfo = [],
            Contents = [new Content { Name = "Body", Html = "Public body" }]
        };

        PageRole[] roles =
        [
            new()
            {
                PageId = page.Id,
                RoleId = guestRole.Id,
                Role = guestRole
            }
        ];

        appOrchestrationServiceMock.Setup(expression: x => x.GetAllApp())
            .Returns(value: new[] { app }.AsQueryable());

        layoutOrchestrationServiceMock.Setup(expression: x => x.GetAllLayout())
            .Returns(value: app.Layouts.AsQueryable());

        pageOrchestrationServiceMock.Setup(expression: x => x.GetAllPage(ignoreFilters: true))
            .Returns(value: new[] { page }.AsQueryable());

        pageOrchestrationServiceMock.Setup(expression: x => x.GetAllPage())
            .Returns(value: new[] { page }.AsQueryable());

        pageRoleOrchestrationServiceMock.Setup(expression: x => x.GetAllPageRole(ignoreFilters: true))
            .Returns(value: roles.AsQueryable());

        SetupRenderResult(renderResult: CreateRenderResult(bodyHtml: "Public body"));

        // When
        aggregationService.RenderRenderResult(appId: app.Id, path: string.Empty, theme: "Default", culture: string.Empty);

        // Then
        pageRenderOrchestrationServiceMock.Verify(
            expression: service => service.ProcessPageRenderOperation(
                operation: It.Is<PageRenderOperation>(match: operation =>
                    operation.OperationType != PageRenderOperationType.UserCanPage
                    && operation.SourcePage.Contents.Any(predicate: content => content.Html == "Public body")
                    && operation.SourcePage.Roles.Any(predicate: role =>
                        role.Role != null
                        && role.Role.Privileges != null
                        && role.Role.Privileges.Contains(item: "page_read")))),
            times: Times.Once);

        pageRenderOrchestrationServiceMock.Verify(
            expression: service => service.ProcessPageRenderOperation(
                operation: It.Is<PageRenderOperation>(match: operation =>
                    operation.SourcePage.Contents.Any(
                        predicate: content => content.Html == "[component[login]]"))),
            times: Times.Never);
    }

    [Fact]
    public void ShouldUseBodySlotForMissingPageAndGatedPageFallbacks()
    {
        // Given
        App app = CreateApp();
        currentUser = TestUsers.WithoutPrivileges();

        pageRenderOrchestrationServiceMock.Setup(expression: x => x.IsAdminOfApp(appId: app.Id))
            .Returns(value: false);

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

        appOrchestrationServiceMock.Setup(expression: x => x.GetAllApp())
            .Returns(value: new[] { app }.AsQueryable());

        layoutOrchestrationServiceMock.Setup(expression: x => x.GetAllLayout())
            .Returns(value: app.Layouts.AsQueryable());

        pageOrchestrationServiceMock.SetupSequence(expression: x => x.GetAllPage(ignoreFilters: true))
            .Returns(value: Array.Empty<Page>()
            .AsQueryable())
            .Returns(value: new[] { protectedPage }.AsQueryable());

        pageOrchestrationServiceMock.Setup(expression: x => x.GetAllPage())
            .Returns(value: new[] { protectedPage }.AsQueryable());

        SetupRenderResult(renderResult: CreateRenderResult());

        aggregationService.RenderRenderResult(appId: app.Id, path: "missing", theme: "Default", culture: string.Empty);
        // When
        aggregationService.RenderRenderResult(appId: app.Id, path: "Admin", theme: "Default", culture: string.Empty);

        // Then
        pageRenderOrchestrationServiceMock.Verify(
            expression: service => service.ProcessPageRenderOperation(
                operation: It.Is<PageRenderOperation>(match: operation =>
                    operation.SourcePage.Path == "missing"
                    && operation.SourcePage.Contents.Any(
                        predicate: content =>
                            content.Name == "body"
                            && content.Html == "[component[NotFound]]"))),
            times: Times.Once);

        pageRenderOrchestrationServiceMock.Verify(
            expression: service => service.ProcessPageRenderOperation(
                operation: It.Is<PageRenderOperation>(match: operation =>
                    operation.SourcePage.Path == "Admin"
                    && operation.SourcePage.Contents.Any(
                        predicate: content =>
                            content.Name == "body"
                            && content.Html == "[component[login]]"))),
            times: Times.Once);
    }

    [Fact]
    public void ShouldThrowSecurityExceptionWhenAppIsUnknown()
    {
        // Given
        appOrchestrationServiceMock.Setup(expression: x => x.GetAllApp())
            .Returns(value: Array.Empty<App>()
            .AsQueryable());

        // When
        Action act = () => aggregationService.RenderRenderResult(appId: 1, path: string.Empty, theme: "Default", culture: string.Empty);

        // Then
        act.Should()
            .Throw<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Unknown Domain!");
    }
}