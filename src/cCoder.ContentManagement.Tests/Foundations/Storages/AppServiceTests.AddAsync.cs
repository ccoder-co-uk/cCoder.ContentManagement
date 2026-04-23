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
using System.Security;



using FluentAssertions;
using Moq;
using Xunit;
using CmsDataModels = cCoder.Data.Models.CMS;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class AppServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForAddAsync()
    {
        // Given
        App app = CreateRandomApp(id: 0);
        Guid roleId = Guid.NewGuid();
        app.Roles =
        [
            new Role
            {
                Id = roleId,
                Name = "Administrators",
                Privs = "app_admin",
                Users = [new UserRole { RoleId = roleId, UserId = "paul" }],
            },
        ];

        CmsDataModels.App submitted = null;

        authorizationBrokerMock.Setup(x => x.Authorize(It.Is<int?>(appId => appId == null), "App_create"));

        appBrokerMock
            .Setup(x => x.AddAppAsync(It.IsAny<CmsDataModels.App>()))
            .Callback<CmsDataModels.App>(candidate => submitted = candidate)
            .ReturnsAsync((CmsDataModels.App value) => value);
        // When
        App result = await appService.AddAsync(app);

        // Then
        result.Should().BeSameAs(app);
        submitted.Should().NotBeNull();
        submitted.Should().NotBeSameAs(app);
        result.Should().NotBeSameAs(submitted);
        submitted.Should().BeEquivalentTo(new
        {
            app.Id,
            app.DefaultCultureId,
            app.TenantId,
            app.Name,
            app.Domain,
            app.DefaultTheme,
            app.ConfigJson
        });
        submitted.Roles.Should().BeNull();
        result.Should().BeEquivalentTo(new
        {
            app.Id,
            app.DefaultCultureId,
            app.TenantId,
            app.Name,
            app.Domain,
            app.DefaultTheme,
            app.ConfigJson
        });
        result.Roles.Should().BeEquivalentTo(app.Roles);

        appBrokerMock.Verify(
            x => x.AddAppAsync(It.IsAny<CmsDataModels.App>()),
            Times.Once
        );
        appBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize(It.Is<int?>(appId => appId == null), "App_create"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksCreatePrivilegeForAddAsync()
    {
        // Given
        App app = CreateRandomApp(id: 0);

        authorizationBrokerMock
            .Setup(x => x.Authorize(It.Is<int?>(appId => appId == null), "App_create"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await appService.AddAsync(app);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        appBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize(It.Is<int?>(appId => appId == null), "App_create"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}

















