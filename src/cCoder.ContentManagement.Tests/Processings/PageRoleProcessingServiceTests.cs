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
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.ContentManagement.Services.Processings;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.Data;
using Moq;
using IAuthorizationManager = cCoder.ContentManagement.Exposures.IAuthorizationManager;
using IRoleBroker = cCoder.ContentManagement.Brokers.IRoleBroker;
using LocalRole = cCoder.Data.Models.Security.Role;

using cCoder.ContentManagement.Exposures;
using cCoder.ContentManagement.Models;

namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PageRoleProcessingServiceTests
{
    private readonly Mock<IPageBroker> pageBrokerMock = new();
    private User currentUser = TestUsers.WithoutPrivileges();
    private readonly Mock<IPageRoleService> pageRoleServiceMock = new();
    private readonly Mock<IRoleBroker> roleBrokerMock = new();
    private readonly Mock<IAuthorizationManager> authorizationManagerMock = new();
    private readonly PageRoleProcessingService pageRoleProcessingService;

    public PageRoleProcessingServiceTests()
    {
        authorizationManagerMock
            .Setup(expression: manager => manager.GetCurrentUser())
            .Returns(valueFunction: () => currentUser);

        authorizationManagerMock
            .Setup(expression: manager => manager.IsAdminOfApp(
                appId: It.IsAny<int>()))
            .Returns(valueFunction: (int appId) =>
                currentUser?.IsAdminOfApp(appId: appId) ?? false);

        authorizationManagerMock
            .Setup(expression: manager => manager.UserCanPageAuthorization(
                pageAuthorization: It.IsAny<PageAuthorization>()))
            .Returns(valueFunction: (PageAuthorization authorization) =>
                TestUsers.UserCanPage(authorization: authorization));

        pageRoleProcessingService = new PageRoleProcessingService(
service: pageRoleServiceMock.Object,
roleBroker: roleBrokerMock.Object,
pageBroker: pageBrokerMock.Object,
authorizationManager: authorizationManagerMock.Object
        );
    }

    private static LocalRole ToLocalRole(Role role) =>
        new()
        {
            Id = role.Id,
            AppId = role.AppId,
            Name = role.Name,
            Description = role.Description,
            Privs = role.Privs,
        };
}