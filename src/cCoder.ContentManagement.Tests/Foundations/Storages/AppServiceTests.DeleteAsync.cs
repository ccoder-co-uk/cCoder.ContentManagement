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
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForDeleteAsync()
    {
        // Given
        App app = CreateRandomApp(id: 5);
        app.Roles = [new Role { Id = Guid.NewGuid(), AppId = app.Id, Users = [] }];

        appBrokerMock.Setup(x => x.GetAllApps(true)).Returns(new[] { app }.AsQueryable());

        authorizationBrokerMock.Setup(x => x.Authorize((int?)app.Id, "App_delete"));
        appBrokerMock.Setup(x => x.DeleteAppAggregateAsync(It.IsAny<CmsDataModels.App>())).Returns(ValueTask.CompletedTask);

        // When
        await appService.DeleteAsync(5);

        // Then
        appBrokerMock.Verify(x => x.GetAllApps(true), Times.Once);
        appBrokerMock.Verify(x => x.DeleteAppAggregateAsync(It.Is<CmsDataModels.App>(actual => actual.Id == app.Id)), Times.Once);
        appBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)app.Id, "App_delete"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        App app = CreateRandomApp(id: 5);
        app.Roles = [new Role { Id = Guid.NewGuid(), AppId = app.Id, Users = [] }];

        appBrokerMock.Setup(x => x.GetAllApps(true)).Returns(new[] { app }.AsQueryable());

        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)app.Id, "App_delete"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await appService.DeleteAsync(5);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        appBrokerMock.Verify(x => x.GetAllApps(true), Times.Once);
        appBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)app.Id, "App_delete"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}

















