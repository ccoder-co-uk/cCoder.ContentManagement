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
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;

using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.ContentManagement.Services.Processings;
using FizzWare.NBuilder;
using Moq;
using IAuthorizationManager = cCoder.ContentManagement.Exposures.IAuthorizationManager;


using cCoder.ContentManagement.Exposures;

namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class AppProcessingServiceTests
{
    private User currentUser = TestUsers.WithoutPrivileges();
    private readonly Mock<IAppService> appServiceMock = new();
    private Mock<IAppService> cultureBrokerMock => appServiceMock;
    private Mock<IAppService> privilegeBrokerMock => appServiceMock;
    private Mock<IAppService> authorizationManagerMock => appServiceMock;
    private Mock<IAppService> roleBrokerMock => appServiceMock;
    private Mock<IAppService> userRoleBrokerMock => appServiceMock;
    private Mock<IAppService> pageBrokerMock => appServiceMock;
    private readonly AppProcessingService appProcessingService;

    public AppProcessingServiceTests()
    {
        appServiceMock.Setup(expression: x => x.GetAllRolesIgnoringFilters())
            .Returns(value: Array.Empty<Role>()
            .AsQueryable());

        appServiceMock.Setup(expression: x => x.GetAllUserRolesIgnoringFilters())
            .Returns(value: Array.Empty<UserRole>()
            .AsQueryable());

        appProcessingService = new AppProcessingService(
service: appServiceMock.Object
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

    private void VerifyNoOtherAppServiceCalls()
    {
        appServiceMock.Verify(
            expression: service => service.GetCurrentUser(),
            times: Times.AtMostOnce());

        appServiceMock.Verify(
            expression: service => service.GetCurrentUserId(),
            times: Times.AtMostOnce());

        appServiceMock.Verify(
            expression: service => service.Authorize(
                It.IsAny<int?>(),
                It.IsAny<string>()),
            times: Times.AtMostOnce());

        appServiceMock.Verify(
            expression: service => service.GetAllRolesIgnoringFilters(),
            times: Times.AtMostOnce());

        appServiceMock.Verify(
            expression: service => service.GetAllUserRolesIgnoringFilters(),
            times: Times.AtMostOnce());

        appServiceMock.Verify(
            expression: service => service.AddRoleAsync(It.IsAny<Role>()),
            times: Times.AtMost(4));

        appServiceMock.Verify(
            expression: service => service.AddUserRoleAsync(It.IsAny<UserRole>()),
            times: Times.AtMost(4));

        appServiceMock.VerifyNoOtherCalls();
    }
}