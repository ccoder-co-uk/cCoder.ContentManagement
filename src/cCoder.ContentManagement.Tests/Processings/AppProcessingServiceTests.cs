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
using cCoder.ContentManagement.Models;
using FizzWare.NBuilder;
using Moq;
using IAuthorizationManager = cCoder.ContentManagement.Exposures.IAuthorizationManager;


using cCoder.ContentManagement.Exposures;

namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class AppProcessingServiceTests
{
    private User currentUser = TestUsers.WithoutPrivileges();
    private readonly Mock<ILegacyAppServiceTestDouble> appServiceMock = new();
    private Mock<ILegacyAppServiceTestDouble> cultureBrokerMock => appServiceMock;
    private Mock<ILegacyAppServiceTestDouble> privilegeBrokerMock => appServiceMock;
    private Mock<ILegacyAppServiceTestDouble> authorizationManagerMock => appServiceMock;
    private Mock<ILegacyAppServiceTestDouble> roleBrokerMock => appServiceMock;
    private Mock<ILegacyAppServiceTestDouble> userRoleBrokerMock => appServiceMock;
    private Mock<ILegacyAppServiceTestDouble> pageBrokerMock => appServiceMock;
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
            service: new AppServiceTestAdapter(service: appServiceMock.Object)
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

    public interface ILegacyAppServiceTestDouble
    {
        App GetApp(int appId, bool ignoreFilters = false);
        ValueTask<App> GetAppForRenderAsync(int appId);
        App GetAppForDelete(int appId);
        IQueryable<App> GetAllApp(bool ignoreFilters = false);
        ValueTask<App> AddAppAsync(App newApp);
        ValueTask<App> UpdateAppAsync(App updatedApp);
        ValueTask DeleteAsync(int appId);
        string GetRequestPath();
        string GetRequestHost();
        IQueryable<Culture> GetAllCultures();
        IQueryable<Privilege> GetAllPrivileges();
        User GetCurrentUser();
        string GetCurrentUserId();
        bool IsAdminOfApp(int appId);
        void Authorize(int? appId, string privilege);
        ValueTask<Role> AddRoleAsync(Role newRole);
        ValueTask<Role> UpdateRoleAsync(Role updatedRole);
        IQueryable<Role> GetAllRolesIgnoringFilters();
        ValueTask<UserRole> AddUserRoleAsync(UserRole newUserRole);
        IQueryable<UserRole> GetAllUserRolesIgnoringFilters();
        ValueTask DeleteAllUserRolesAsync(IEnumerable<UserRole> deletedUserRole);
        IQueryable<Page> GetAllPagesIgnoringFilters();
        ValueTask<Page> UpdatePageAsync(Page updatedPage);
    }

    private sealed class AppServiceTestAdapter(ILegacyAppServiceTestDouble service) : IAppService
    {
        public AppOperation GetVisibleAppsAppOperation(AppOperation appOperation)
        {
            appOperation.Apps = service.GetAllApp();
            return appOperation;
        }

        public AppOperation GetUnfilteredAppsAppOperation(AppOperation appOperation)
        {
            appOperation.Apps = service.GetAllApp(ignoreFilters: true);
            return appOperation;
        }

        public AppOperation GetVisibleAppAppOperation(AppOperation appOperation)
        {
            appOperation.App = service.GetApp(appId: appOperation.AppId);
            return appOperation;
        }

        public AppOperation GetUnfilteredAppAppOperation(AppOperation appOperation)
        {
            appOperation.App = service.GetApp(appId: appOperation.AppId, ignoreFilters: true);
            return appOperation;
        }

        public async ValueTask<AppOperation> GetAppForRenderAppOperationAsync(AppOperation appOperation)
        {
            appOperation.App = await service.GetAppForRenderAsync(appId: appOperation.AppId);
            return appOperation;
        }

        public AppOperation GetAppForDeleteAppOperation(AppOperation appOperation)
        {
            appOperation.App = new App { Id = appOperation.AppId };
            return appOperation;
        }

        public async ValueTask<AppOperation> AddAppOperationAsync(AppOperation newAppOperation)
        {
            newAppOperation.App = await service.AddAppAsync(newApp: newAppOperation.App);
            return newAppOperation;
        }

        public async ValueTask<AppOperation> UpdateAppOperationAsync(AppOperation updatedAppOperation)
        {
            updatedAppOperation.App = await service.UpdateAppAsync(updatedApp: updatedAppOperation.App);
            return updatedAppOperation;
        }

        public async ValueTask<AppOperation> DeleteAppOperationAsync(AppOperation deletedAppOperation)
        {
            await service.DeleteAsync(appId: deletedAppOperation.App.Id);
            return deletedAppOperation;
        }

        public AppOperation GetRequestPathAppOperation(AppOperation appOperation) { appOperation.Text = service.GetRequestPath(); return appOperation; }
        public AppOperation GetRequestHostAppOperation(AppOperation appOperation) { appOperation.Text = service.GetRequestHost(); return appOperation; }
        public AppOperation GetCulturesAppOperation(AppOperation appOperation) { appOperation.Cultures = service.GetAllCultures(); return appOperation; }
        public AppOperation GetPrivilegesAppOperation(AppOperation appOperation) { appOperation.Privileges = service.GetAllPrivileges(); return appOperation; }
        public AppOperation GetCurrentUserAppOperation(AppOperation appOperation) { appOperation.Text = service.GetCurrentUser()?.Id; return appOperation; }
        public AppOperation GetCurrentUserIdAppOperation(AppOperation appOperation) { appOperation.Text = service.GetCurrentUserId(); return appOperation; }
        public AppOperation IsAdminOfAppAppOperation(AppOperation appOperation) { appOperation.Result = service.IsAdminOfApp(appId: appOperation.AppId); return appOperation; }
        public AppOperation AuthorizeAppOperation(AppOperation appOperation) { service.Authorize(appId: appOperation.OptionalAppId, privilege: appOperation.Privilege); return appOperation; }
        public async ValueTask<AppOperation> AddRoleAppOperationAsync(AppOperation newAppOperation) { newAppOperation.Role = await service.AddRoleAsync(newRole: newAppOperation.Role); return newAppOperation; }
        public async ValueTask<AppOperation> UpdateRoleAppOperationAsync(AppOperation updatedAppOperation) { updatedAppOperation.Role = await service.UpdateRoleAsync(updatedRole: updatedAppOperation.Role); return updatedAppOperation; }
        public AppOperation GetRolesAppOperation(AppOperation appOperation) { appOperation.Roles = service.GetAllRolesIgnoringFilters(); return appOperation; }
        public async ValueTask<AppOperation> AddUserRoleAppOperationAsync(AppOperation newAppOperation) { newAppOperation.UserRole = await service.AddUserRoleAsync(newUserRole: newAppOperation.UserRole); return newAppOperation; }
        public AppOperation GetUserRolesAppOperation(AppOperation appOperation) { appOperation.UserRoles = service.GetAllUserRolesIgnoringFilters(); return appOperation; }
        public async ValueTask<AppOperation> DeleteUserRolesAppOperationAsync(AppOperation deletedAppOperation) { await service.DeleteAllUserRolesAsync(deletedUserRole: deletedAppOperation.DeletedUserRoles); return deletedAppOperation; }
        public AppOperation GetPagesAppOperation(AppOperation appOperation) { appOperation.Pages = service.GetAllPagesIgnoringFilters(); return appOperation; }
        public async ValueTask<AppOperation> UpdatePageAppOperationAsync(AppOperation updatedAppOperation) { updatedAppOperation.Page = await service.UpdatePageAsync(updatedPage: updatedAppOperation.Page); return updatedAppOperation; }
    }
}