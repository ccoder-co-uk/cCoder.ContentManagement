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
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Services.Processings;
using cCoder.ContentManagement.Models;
using FizzWare.NBuilder;
using Moq;

namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class PageRoleOrchestrationServiceTests
{
    private readonly Mock<IPageRoleProcessingService> pageRoleProcessingServiceMock;
    private readonly Mock<IPageRoleEventProcessingService> pageRoleEventProcessingServiceMock;
    private readonly Mock<IAuthorizationProcessingService> authorizationProcessingServiceMock;
    private readonly PageRoleOrchestrationService orchestrationService;
    private const string CurrentUserId = "test-user";

    public PageRoleOrchestrationServiceTests()
    {
        pageRoleProcessingServiceMock = new Mock<IPageRoleProcessingService>(behavior: MockBehavior.Strict);
        pageRoleEventProcessingServiceMock = new Mock<IPageRoleEventProcessingService>(behavior: MockBehavior.Strict);
        authorizationProcessingServiceMock = new(behavior: MockBehavior.Strict);

        authorizationProcessingServiceMock
            .Setup(expression: service => service.GetCurrentUserId())
            .Returns(value: CurrentUserId);

        orchestrationService = new PageRoleOrchestrationService(
            processingService: pageRoleProcessingServiceMock.Object,
            eventService: pageRoleEventProcessingServiceMock.Object,
            authorizationProcessingService:
                authorizationProcessingServiceMock.Object
        );
    }

    private static PageRole CreateRandomPageRole() =>
        Builder<PageRole>.CreateNew()
        .Build();

    private void SetupAuthorization(
        PageRole pageRole,
        string privilege,
        bool allowed,
        bool requireRole = false)
    {
        Page page = new()
        {
            Id = pageRole.PageId,
            AppId = 42
        };

        Role role = requireRole
            ? new Role
            {
                Id = pageRole.RoleId,
                AppId = page.AppId
            }
            : null;

        pageRoleProcessingServiceMock
            .Setup(expression: service =>
                service.ResolvePageRole(
                    pageRole: It.Is<PageRole>(match: match =>
                        match.PageId == pageRole.PageId
                        && match.RoleId == pageRole.RoleId)))
            .Returns(value: new PageRole
            {
                PageId = pageRole.PageId,
                RoleId = pageRole.RoleId,
                Page = page,
                Role = role
            });

        authorizationProcessingServiceMock
            .Setup(expression: service =>
                service.ResolveCurrentAuthorizationContext(
                    context: It.Is<AuthorizationContext>(match: match =>
                        match.PageAuthorization.Page == page
                        && match.PageAuthorization.Privilege == privilege)))
            .Returns(valueFunction: (AuthorizationContext context) =>
            {
                context.User = new User();
                return context;
            });

        authorizationProcessingServiceMock
            .Setup(expression: service =>
                service.UserCanPageAuthorizationContext(
                    context: It.Is<AuthorizationContext>(match: match =>
                        match.PageAuthorization.Page == page
                        && match.PageAuthorization.Privilege == privilege)))
            .Returns(value: allowed);

        if (allowed)
        {
            authorizationProcessingServiceMock
                .Setup(expression: service =>
                    service.AuthorizeAuthorizationContext(
                        context: It.Is<AuthorizationContext>(match: match =>
                            match.Request.AppId == page.AppId
                            && match.Request.Privilege == privilege)));
        }
    }
}
