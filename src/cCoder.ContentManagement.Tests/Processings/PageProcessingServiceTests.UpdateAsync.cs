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
using System.Security;

using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PageProcessingServiceTests
{
    [Fact]
    public async Task ShouldUpdatePageWhenUserCanUpdatePageForUpdateAsync()
    {

        // Given
        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: It.IsAny<int?>(), privilege: It.IsAny<string>()))
            .Callback(action: (int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId: appId, operation: privilege) ?? false))
                {
                    throw new SecurityException(message: "Access Denied!");
                }
            });

        authorizationBrokerMock
            .Setup(expression: x => x.IsAdminOfApp(appId: It.IsAny<int>()))
            .Returns(valueFunction: (int appId) => currentUser?.IsAdminOfApp(appId: appId) ?? false);

        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(valueFunction: () => currentUser);

        User actor = TestUsers.WithPrivilege(privilege: "app_admin", appId: 1);

        PageInfo pageInfo = new()
        {
            CultureId = string.Empty,
            Title = "Home",
            Description = "Home",
            Keywords = "Home",
        };

        Page dbPage = CreateRandomPage();
        Page page = CreateRandomPage();
        dbPage.AppId = 1;
        page.Id = dbPage.Id;
        page.AppId = dbPage.AppId;
        page.Name = dbPage.Name;
        page.Path = dbPage.Path;
        page.ParentId = dbPage.ParentId;
        page.PageInfo = [pageInfo];
        page.Contents = [];
        page.Roles = [];

        dbPage.PageInfo =
        [
            new PageInfo
            {
                CultureId = string.Empty,
                Title = "Home",
                Description = "Home",
                Keywords = "Home",
            },
        ];

        dbPage.Contents = [];
        dbPage.Roles = [];

        currentUser = actor;

        pageServiceMock.Setup(expression: x => x.GetAllPage(ignoreFilters: true))
            .Returns(value: new[] { dbPage }.AsQueryable());

        pageServiceMock.Setup(expression: x => x.GetAllPage(ignoreFilters: false))
            .Returns(value: new[] { dbPage }.AsQueryable());

        pageServiceMock.Setup(expression: x => x.UpdatePageAsync(updatedPage: It.IsAny<Page>()))
            .ReturnsAsync(value: dbPage);

        // When
        Page result = await pageProcessingService.UpdatePageAsync(updatedPage: page);

        // Then
        result.Should()
            .BeSameAs(expected: dbPage);

        pageServiceMock.Verify(expression: x => x.GetAllPage(ignoreFilters: false), times: Times.Once);
        pageServiceMock.Verify(expression: x => x.GetAllPage(ignoreFilters: true), times: Times.Once);

        pageServiceMock.Verify(expression: x => x.UpdatePageAsync(updatedPage: It.Is<Page>(match: updated =>
            updated.Id == page.Id &&
            updated.AppId == page.AppId &&
            updated.Name == page.Name)), times: Times.Once);

        pageServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserCannotUpdatePageForUpdateAsync()
    {
        // Given
        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: It.IsAny<int?>(), privilege: It.IsAny<string>()))
            .Callback(action: (int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId: appId, operation: privilege) ?? false))
                {
                    throw new SecurityException(message: "Access Denied!");
                }
            });

        authorizationBrokerMock
            .Setup(expression: x => x.IsAdminOfApp(appId: It.IsAny<int>()))
            .Returns(valueFunction: (int appId) => currentUser?.IsAdminOfApp(appId: appId) ?? false);

        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(valueFunction: () => currentUser);

        Page page = CreateRandomPage();

        pageServiceMock.Setup(expression: x => x.GetAllPage(ignoreFilters: true))
            .Returns(value: new[] { page }.AsQueryable());

        pageServiceMock.Setup(expression: x => x.GetAllPage(ignoreFilters: false))
            .Returns(value: new[] { page }.AsQueryable());

        // When
        Func<Task> act = async () => await pageProcessingService.UpdatePageAsync(updatedPage: page);

        // Then
        await act.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        pageServiceMock.Verify(expression: x => x.GetAllPage(ignoreFilters: false), times: Times.Once);
        pageServiceMock.Verify(expression: x => x.GetAllPage(ignoreFilters: true), times: Times.Once);
        pageServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenNewParentCannotBeResolvedForUpdateAsync()
    {

        // Given
        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: It.IsAny<int?>(), privilege: It.IsAny<string>()))
            .Callback(action: (int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId: appId, operation: privilege) ?? false))
                {
                    throw new SecurityException(message: "Access Denied!");
                }
            });

        authorizationBrokerMock
            .Setup(expression: x => x.IsAdminOfApp(appId: It.IsAny<int>()))
            .Returns(valueFunction: (int appId) => currentUser?.IsAdminOfApp(appId: appId) ?? false);

        authorizationBrokerMock.Setup(expression: x => x.GetCurrentUser())
            .Returns(valueFunction: () => currentUser);

        User actor = TestUsers.WithPrivilege(privilege: "app_admin", appId: 1);
        Page dbPage = CreateRandomPage();
        Page page = CreateRandomPage();
        dbPage.AppId = 1;
        int missingParentId = dbPage.Id + 1000;

        dbPage.PageInfo =
        [
            new PageInfo
            {
                CultureId = string.Empty,
                Title = "Home",
                Description = "Home",
                Keywords = "Home",
            },
        ];

        dbPage.Contents = [];
        dbPage.Roles = [];
        dbPage.AppId = 1;
        page.Id = dbPage.Id;
        page.AppId = dbPage.AppId;
        page.Name = dbPage.Name;
        page.Path = dbPage.Path;
        page.ParentId = missingParentId;

        page.PageInfo =
        [
            new PageInfo
            {
                CultureId = string.Empty,
                Title = "Home",
                Description = "Home",
                Keywords = "Home",
            },
        ];

        page.Contents = [];
        page.Roles = [];

        currentUser = actor;

        pageServiceMock.SetupSequence(expression: x => x.GetAllPage(ignoreFilters: true))
            .Returns(value: new[] { dbPage }.AsQueryable())
            .Returns(value: Array.Empty<Page>()
            .AsQueryable());

        pageServiceMock.Setup(expression: x => x.GetAllPage(ignoreFilters: false))
            .Returns(value: new[] { dbPage }.AsQueryable());

        // When
        Func<Task> act = async () => await pageProcessingService.UpdatePageAsync(updatedPage: page);

        // Then
        await act.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        pageServiceMock.Verify(expression: x => x.GetAllPage(ignoreFilters: false), times: Times.Once);
        pageServiceMock.Verify(expression: x => x.GetAllPage(ignoreFilters: true), times: Times.Exactly(callCount: 2));
        pageServiceMock.VerifyNoOtherCalls();
    }

}