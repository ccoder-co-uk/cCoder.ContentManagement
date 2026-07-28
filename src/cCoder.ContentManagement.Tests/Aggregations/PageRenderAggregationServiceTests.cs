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
using cCoder.ContentManagement.Services.Coordinations;
using cCoder.ContentManagement.Services.Aggregations;
using cCoder.ContentManagement.Extensions;
using cCoder.ContentManagement.Services.Orchestrations;
using Moq;
using IAuthorizationBroker = cCoder.ContentManagement.Brokers.IAuthorizationBroker;
using IPageRenderOrchestrationService = cCoder.ContentManagement.Services.Orchestrations.IPageRenderOrchestrationService;

namespace cCoder.Core.Services.Tests.CMS.Aggregations;

public partial class PageRenderAggregationServiceTests
{
    private User currentUser = TestUsers.WithoutPrivileges();
    private readonly Mock<IAppOrchestrationService> appOrchestrationServiceMock = new();
    private readonly Mock<ILayoutOrchestrationService> layoutOrchestrationServiceMock = new();
    private readonly Mock<ITemplateOrchestrationService> templateOrchestrationServiceMock = new();
    private readonly Mock<IResourceOrchestrationService> resourceOrchestrationServiceMock = new();
    private readonly Mock<IComponentOrchestrationService> componentOrchestrationServiceMock = new();
    private readonly Mock<IScriptOrchestrationService> scriptOrchestrationServiceMock = new();
    private readonly Mock<IPageOrchestrationService> pageOrchestrationServiceMock = new();
    private readonly Mock<IContentOrchestrationService> contentOrchestrationServiceMock = new();
    private readonly Mock<IPageInfoOrchestrationService> pageInfoOrchestrationServiceMock = new();
    private readonly Mock<IPageRoleOrchestrationService> pageRoleOrchestrationServiceMock = new();
    private readonly Mock<IPageRenderOrchestrationService> pageRenderOrchestrationServiceMock = new();
    private readonly PageRenderAggregationService aggregationService;

    public PageRenderAggregationServiceTests()
    {
        pageRenderOrchestrationServiceMock
            .Setup(expression: service => service.ResolveCulture(
                culture: It.IsAny<string>()))
            .Returns(valueFunction: (string culture) =>
                culture ?? currentUser.DefaultCultureId);

        pageRenderOrchestrationServiceMock
            .Setup(expression: service => service.ProcessPageRenderOperation(
                operation: It.Is<PageRenderOperation>(match: operation =>
                    operation.OperationType == PageRenderOperationType.UserCanPage)))
            .Returns(valueFunction: (PageRenderOperation operation) =>
            {
                operation.IsAuthorized = ContentManagementModelExtensions.UserCan(
                    page: operation.SourcePage,
                    user: currentUser,
                    privilege: operation.Privilege);

                return operation;
            });

        layoutOrchestrationServiceMock.Setup(expression: x => x.GetAllLayout(ignoreFilters: false))
            .Returns(value: Array.Empty<Layout>()
            .AsQueryable());

        templateOrchestrationServiceMock.Setup(expression: x => x.GetAllTemplate(ignoreFilters: false))
            .Returns(value: Array.Empty<Template>()
            .AsQueryable());

        resourceOrchestrationServiceMock.Setup(expression: x => x.GetAllResource(ignoreFilters: false))
            .Returns(value: Array.Empty<Resource>()
            .AsQueryable());

        componentOrchestrationServiceMock.Setup(expression: x => x.GetAllComponent(ignoreFilters: false))
            .Returns(value: Array.Empty<Component>()
            .AsQueryable());

        scriptOrchestrationServiceMock.Setup(expression: x => x.GetAllScript(ignoreFilters: false))
            .Returns(value: Array.Empty<Script>()
            .AsQueryable());

        pageOrchestrationServiceMock.Setup(expression: x => x.GetAllPage(ignoreFilters: false))
            .Returns(value: Array.Empty<Page>()
            .AsQueryable());

        pageOrchestrationServiceMock.Setup(expression: x => x.GetAllPage(ignoreFilters: true))
            .Returns(value: Array.Empty<Page>()
            .AsQueryable());

        contentOrchestrationServiceMock.Setup(expression: x => x.GetAllContent(ignoreFilters: true))
            .Returns(value: Array.Empty<Content>()
            .AsQueryable());

        pageInfoOrchestrationServiceMock.Setup(expression: x => x.GetAllPageInfo(ignoreFilters: true))
            .Returns(value: Array.Empty<PageInfo>()
            .AsQueryable());

        pageRoleOrchestrationServiceMock.Setup(expression: x => x.GetAllPageRole(ignoreFilters: true))
            .Returns(value: Array.Empty<PageRole>()
            .AsQueryable());

        aggregationService = new PageRenderAggregationService(
appOrchestrationService: appOrchestrationServiceMock.Object,
layoutOrchestrationService: layoutOrchestrationServiceMock.Object,
templateOrchestrationService: templateOrchestrationServiceMock.Object,
resourceOrchestrationService: resourceOrchestrationServiceMock.Object,
componentOrchestrationService: componentOrchestrationServiceMock.Object,
scriptOrchestrationService: scriptOrchestrationServiceMock.Object,
pageOrchestrationService: pageOrchestrationServiceMock.Object,
contentOrchestrationService: contentOrchestrationServiceMock.Object,
pageInfoOrchestrationService: pageInfoOrchestrationServiceMock.Object,
pageRoleOrchestrationService: pageRoleOrchestrationServiceMock.Object,
pageRenderOrchestrationService: pageRenderOrchestrationServiceMock.Object);
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
            Layouts =
            [
                new Layout
                {
                    Id = 1,
                    AppId = 1,
                    Name = "Default",
                    HeaderHtml = "<title>[page[title]]</title>",
                    Html = "<main>[content[Body]]</main>",
                    Script = string.Empty
                }
            ]
        };

    private static RenderResult CreateRenderResult(string bodyHtml = "Body") =>
        new()
        {
            HeaderHtml = "Header",
            BodyHtml = bodyHtml,
            Theme = "Ocean",
            Culture = "en-GB",
            Edit = false,
            StatusCode = 200
        };

    private static PageRenderOperation CreatePageRenderOperation(RenderResult renderResult) =>
        new()
        {
            Page = renderResult
        };

    private void SetupRenderResult(RenderResult renderResult) =>
        pageRenderOrchestrationServiceMock
            .Setup(expression: service => service.ProcessPageRenderOperation(
                operation: It.Is<PageRenderOperation>(match: operation =>
                    operation.OperationType != PageRenderOperationType.UserCanPage)))
            .Returns(valueFunction: (PageRenderOperation operation) =>
            {
                operation.Page = renderResult;

                return operation;
            });
}