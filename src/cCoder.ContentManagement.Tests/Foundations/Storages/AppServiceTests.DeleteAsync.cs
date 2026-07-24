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

        appBrokerMock.Setup(expression: x => x.GetAllApps(ignoreFilters: true))
            .Returns(value: new[] { app }.AsQueryable());

        authorizationBrokerMock.Setup(expression: x => x.Authorize(appId: (int?)app.Id, privilege: "App_delete"));

        appBrokerMock.Setup(expression: x => x.DeleteAppAggregateAsync(deletedApp: It.IsAny<CmsDataModels.App>()))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await appService.DeleteAsync(appId: 5);

        // Then
        appBrokerMock.Verify(expression: x => x.GetAllApps(ignoreFilters: true), times: Times.Once);
        appBrokerMock.Verify(expression: x => x.DeleteAppAggregateAsync(deletedApp: It.Is<CmsDataModels.App>(match: actual => actual.Id == app.Id)), times: Times.Once);
        appBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)app.Id, privilege: "App_delete"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        App app = CreateRandomApp(id: 5);
        app.Roles = [new Role { Id = Guid.NewGuid(), AppId = app.Id, Users = [] }];

        appBrokerMock.Setup(expression: x => x.GetAllApps(ignoreFilters: true))
            .Returns(value: new[] { app }.AsQueryable());

        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: (int?)app.Id, privilege: "App_delete"))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () => await appService.DeleteAsync(appId: 5);

        // Then

        await action.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        appBrokerMock.Verify(expression: x => x.GetAllApps(ignoreFilters: true), times: Times.Once);
        appBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(expression: x => x.Authorize(appId: (int?)app.Id, privilege: "App_delete"), times: Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}