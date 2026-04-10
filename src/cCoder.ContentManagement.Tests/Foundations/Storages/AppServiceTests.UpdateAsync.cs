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
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForUpdateAsync()
    {
        // Given
        App app = CreateRandomApp(id: 5);

        CmsDataModels.App submitted = null;

        authorizationBrokerMock.Setup(x => x.Authorize((int?)app.Id, "App_update"));

        appBrokerMock
            .Setup(x => x.UpdateAppAsync(It.IsAny<CmsDataModels.App>()))
            .Callback<CmsDataModels.App>(candidate => submitted = candidate)
            .ReturnsAsync((CmsDataModels.App value) => value);

        // When
        App result = await appService.UpdateAsync(app);

        // Then
        result.Should().NotBeSameAs(app);
        submitted.Should().NotBeNull();
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
        result.Roles.Should().BeNull();
        appBrokerMock.Verify(x => x.UpdateAppAsync(It.IsAny<CmsDataModels.App>()), Times.Once);
        appBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)app.Id, "App_update"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksUpdatePrivilegeForUpdateAsync()
    {
        // Given
        App app = CreateRandomApp(id: 5);

        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)app.Id, "App_update"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await appService.UpdateAsync(app);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        appBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)app.Id, "App_update"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}

















