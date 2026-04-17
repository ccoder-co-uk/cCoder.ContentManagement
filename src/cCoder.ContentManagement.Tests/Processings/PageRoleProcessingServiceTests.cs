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
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.ContentManagement.Services.Processings;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.Data;
using Moq;
using IAuthorizationBroker = cCoder.ContentManagement.Brokers.IAuthorizationBroker;
using IRoleBroker = cCoder.ContentManagement.Brokers.IRoleBroker;
using LocalRole = cCoder.Data.Models.Security.Role;

namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PageRoleProcessingServiceTests
{
    private readonly Mock<IPageService> pageServiceMock = new();
    private User currentUser = TestUsers.WithoutPrivileges();
    private readonly Mock<IPageRoleService> pageRoleServiceMock = new();
    private readonly Mock<IPageRoleBroker> pageRoleBrokerMock = new();
    private readonly Mock<IRoleBroker> roleBrokerMock = new();
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock = new();
    private readonly PageRoleProcessingService pageRoleProcessingService;

    public PageRoleProcessingServiceTests()
    {
        pageRoleProcessingService = new PageRoleProcessingService(
            pageRoleServiceMock.Object,
            pageRoleBrokerMock.Object,
            roleBrokerMock.Object,
            pageServiceMock.Object,
            authorizationBrokerMock.Object
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





























