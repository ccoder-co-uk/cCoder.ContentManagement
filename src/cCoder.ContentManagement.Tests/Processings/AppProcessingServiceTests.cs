// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;

using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.ContentManagement.Services.Processings;
using FizzWare.NBuilder;
using Moq;
using IAuthorizationBroker = cCoder.ContentManagement.Brokers.IAuthorizationBroker;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class AppProcessingServiceTests
{
    private User currentUser = TestUsers.WithoutPrivileges();
    private readonly Mock<IAppService> appServiceMock = new();
    private readonly Mock<ICultureService> cultureServiceMock = new();
    private readonly Mock<IPrivilegeBroker> privilegeBrokerMock = new();
    private readonly Mock<IAuthorizationBroker> authorizationBrokerMock = new();
    private readonly Mock<IRoleBroker> roleBrokerMock = new();
    private readonly Mock<IUserRoleBroker> userRoleBrokerMock = new();
    private readonly AppProcessingService appProcessingService;

    public AppProcessingServiceTests()
    {
        roleBrokerMock.Setup(expression: x => x.GetAllRoles(ignoreFilters: true))
            .Returns(value: Array.Empty<Role>()
            .AsQueryable());

        userRoleBrokerMock.Setup(expression: x => x.GetAllUserRoles(ignoreFilters: true))
            .Returns(value: Array.Empty<UserRole>()
            .AsQueryable());

        appProcessingService = new AppProcessingService(
service: appServiceMock.Object,
cultureService: cultureServiceMock.Object,
privilegeBroker: privilegeBrokerMock.Object,
authorizationBroker: authorizationBrokerMock.Object,
roleBroker: roleBrokerMock.Object,
userRoleBroker: userRoleBrokerMock.Object
        );
    }

    private static App CreateRandomApp() =>
        Builder<App>
            .CreateNew()
        .With(func: x => x.Id = Random.Shared.Next(minValue: 1, maxValue: 10000))
        .With(func: x => x.DefaultCultureId = string.Empty)
        .With(func: x => x.Name = $"App-{Guid.NewGuid():N}")
        .With(func: x => x.Domain = $"{Guid.NewGuid():N}.local")
        .With(func: x => x.DefaultTheme = "Default")
        .With(func: x => x.ConfigJson = "{}")
        .With(func: x => x.Cultures = [])
        .With(func: x => x.Pages = [])
        .With(func: x => x.Components = [])
        .With(func: x => x.Scripts = [])
        .With(func: x => x.Roles = [])
        .With(func: x => x.Templates = [])
        .With(func: x => x.Resources = [])
        .With(func: x => x.Layouts = [])
        .Build();
}