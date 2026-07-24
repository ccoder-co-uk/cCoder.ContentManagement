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

namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PageProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenAddAsync()
    {
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

        User actor = TestUsers.WithPrivilege(privilege: "page_create", appId: 1);

        Page page = new()
        {
            AppId = 1,
            Name = "About",
            PageInfo = [new PageInfo { CultureId = string.Empty, Title = "About Us" }],
            Contents = [],
        };

        Page addedPage = new()
        {
            Id = 12,
            AppId = 1,
            Name = "About",
            Path = "About",
            Roles = [],
        };

        currentUser = actor;

        pageServiceMock.Setup(expression: x => x.AddPageAsync(newPage: It.IsAny<Page>()))
            .ReturnsAsync(value: addedPage);

        pageServiceMock.Setup(expression: x => x.GetAllPage(ignoreFilters: true))
            .Returns(value: Array.Empty<Page>()
            .AsQueryable());

        Page result = await pageProcessingService.AddPageAsync(newPage: page);

        result.Should()
            .BeSameAs(expected: addedPage);

        pageServiceMock.Verify(
expression: x =>
                x.AddPageAsync(
newPage: It.Is<Page>(match: p =>
                        p.Path == "About"
                        && p.Name == "About"
                        && p.AppId == 1
                        && p.PageInfo.Any(predicate: i => i.CultureId == string.Empty)
                        && p.Contents != null
                        && p.Roles != null
                    )
                ),
times: Times.Once
        );

        pageServiceMock.Verify(expression: x => x.GetAllPage(ignoreFilters: true), times: Times.Once);
        pageServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldUseParentPathWhenAddAsync()
    {
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

        Page parent = new()
        {
            Id = 55,
            AppId = 1,
            Path = "parent",
            Roles = [],
            PageInfo = [new PageInfo { CultureId = string.Empty, Title = "Parent" }],
        };

        Page page = new()
        {
            AppId = 1,
            Name = "Child",
            ParentId = parent.Id,
            PageInfo = [new PageInfo { CultureId = string.Empty, Title = "Child Title" }],
            Contents = [],
            Roles = [],
        };

        Page addedPage = new()
        {
            Id = 99,
            AppId = 1,
            Name = "Child",
            Path = "parent/Child",
            Roles = [],
        };

        currentUser = actor;

        pageServiceMock.Setup(expression: x => x.AddPageAsync(newPage: It.IsAny<Page>()))
            .ReturnsAsync(value: addedPage);

        pageServiceMock.Setup(expression: x => x.GetAllPage(ignoreFilters: false))
            .Returns(value: new[] { parent }.AsQueryable());

        pageServiceMock.Setup(expression: x => x.GetAllPage(ignoreFilters: true))
            .Returns(value: new[] { parent }.AsQueryable());

        Page result = await pageProcessingService.AddPageAsync(newPage: page);

        result.Should()
            .BeSameAs(expected: addedPage);

        pageServiceMock.Verify(expression: x => x.GetAllPage(ignoreFilters: false), times: Times.Once);
        pageServiceMock.Verify(expression: x => x.GetAllPage(ignoreFilters: true), times: Times.Once);

        pageServiceMock.Verify(
expression: x => x.AddPageAsync(newPage: It.Is<Page>(match: p => p.Path == "parent/Child" && p.AppId == 1)),
times: Times.Once
        );

        pageServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowValidationExceptionWhenComputedPathAlreadyExistsForSameAppOnAddAsync()
    {
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

        currentUser = TestUsers.WithPrivilege(privilege: "page_create", appId: 1);

        Page existingPage = new()
        {
            Id = 24,
            AppId = 1,
            Name = "About",
            Path = "About",
        };

        Page page = new()
        {
            AppId = 1,
            Name = "About",
            PageInfo = [new PageInfo { CultureId = string.Empty, Title = "About Us" }],
            Contents = [],
        };

        pageServiceMock.Setup(expression: x => x.GetAllPage(ignoreFilters: true))
            .Returns(value: new[] { existingPage }.AsQueryable());

        Func<Task> act = async () => await pageProcessingService.AddPageAsync(newPage: page);

        await act.Should()
            .ThrowAsync<System.ComponentModel.DataAnnotations.ValidationException>()
            .WithMessage(expectedWildcardPattern: "A page already exists for app 1 with path 'About'.");

        pageServiceMock.Verify(expression: x => x.GetAllPage(ignoreFilters: true), times: Times.Once);
        pageServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldAllowComputedPathWhenItOnlyExistsForAnotherAppOnAddAsync()
    {
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

        currentUser = TestUsers.WithPrivilege(privilege: "page_create", appId: 1);

        Page existingPage = new()
        {
            Id = 24,
            AppId = 2,
            Name = "About",
            Path = "About",
        };

        Page page = new()
        {
            AppId = 1,
            Name = "About",
            PageInfo = [new PageInfo { CultureId = string.Empty, Title = "About Us" }],
            Contents = [],
        };

        Page addedPage = new()
        {
            Id = 12,
            AppId = 1,
            Name = "About",
            Path = "About",
            Roles = [],
        };

        pageServiceMock.Setup(expression: x => x.GetAllPage(ignoreFilters: true))
            .Returns(value: new[] { existingPage }.AsQueryable());

        pageServiceMock.Setup(expression: x => x.AddPageAsync(newPage: It.IsAny<Page>()))
            .ReturnsAsync(value: addedPage);

        Page result = await pageProcessingService.AddPageAsync(newPage: page);

        result.Should()
            .BeSameAs(expected: addedPage);

        pageServiceMock.Verify(expression: x => x.GetAllPage(ignoreFilters: true), times: Times.Once);

        pageServiceMock.Verify(
expression: x => x.AddPageAsync(newPage: It.Is<Page>(match: p => p.AppId == 1 && p.Path == "About")),
times: Times.Once);

        pageServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowValidationExceptionWhenComputedPathExistsOnPageUserCannotSeeForSameAppOnAddAsync()
    {
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

        currentUser = TestUsers.WithPrivilege(privilege: "page_create", appId: 1);
        UserRole userRole = currentUser.Roles.First();

        Page parent = new()
        {
            Id = 55,
            AppId = 1,
            Name = "Parent",
            Path = "parent",
            Roles =
            [
                new PageRole
                {
                    RoleId = userRole.RoleId,
                    Role = userRole.Role,
                },
            ],
            PageInfo = [new PageInfo { CultureId = string.Empty, Title = "Parent" }],
        };

        Page hiddenDuplicate = new()
        {
            Id = 88,
            AppId = 1,
            Name = "Child",
            ParentId = parent.Id,
            Path = "parent/Child",
            Roles = [],
        };

        Page page = new()
        {
            AppId = 1,
            Name = "Child",
            ParentId = parent.Id,
            PageInfo = [new PageInfo { CultureId = string.Empty, Title = "Child Title" }],
            Contents = [],
            Roles = [],
        };

        pageServiceMock.Setup(expression: x => x.GetAllPage(ignoreFilters: false))
            .Returns(value: new[] { parent }.AsQueryable());

        pageServiceMock.Setup(expression: x => x.GetAllPage(ignoreFilters: true))
            .Returns(value: new[] { parent, hiddenDuplicate }.AsQueryable());

        Func<Task> act = async () => await pageProcessingService.AddPageAsync(newPage: page);

        await act.Should()
            .ThrowAsync<System.ComponentModel.DataAnnotations.ValidationException>()
            .WithMessage(expectedWildcardPattern: "A page already exists for app 1 with path 'parent/Child'.");

        pageServiceMock.Verify(expression: x => x.GetAllPage(ignoreFilters: false), times: Times.Exactly(callCount: 2));
        pageServiceMock.Verify(expression: x => x.GetAllPage(ignoreFilters: true), times: Times.Once);
        pageServiceMock.VerifyNoOtherCalls();
    }
}