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
            .Setup(x => x.Authorize(It.IsAny<int?>(), It.IsAny<string>()))
            .Callback((int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId, privilege) ?? false))
                    throw new SecurityException("Access Denied!");
            });

        authorizationBrokerMock
            .Setup(x => x.IsAdminOfApp(It.IsAny<int>()))
            .Returns((int appId) => currentUser?.IsAdminOfApp(appId) ?? false);

        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(() => currentUser);

        User actor = TestUsers.WithPrivilege("page_create", 1);
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
        pageServiceMock.Setup(x => x.AddPageAsync(It.IsAny<Page>())).ReturnsAsync(addedPage);
        pageServiceMock.Setup(x => x.GetAllPage(true)).Returns(Array.Empty<Page>().AsQueryable());

        Page result = await pageProcessingService.AddPageAsync(page);

        result.Should().BeSameAs(addedPage);
        pageServiceMock.Verify(
            x =>
                x.AddPageAsync(
                    It.Is<Page>(p =>
                        p.Path == "About"
                        && p.Name == "About"
                        && p.AppId == 1
                        && p.PageInfo.Any(i => i.CultureId == string.Empty)
                        && p.Contents != null
                        && p.Roles != null
                    )
                ),
            Times.Once
        );
        pageServiceMock.Verify(x => x.GetAllPage(true), Times.Once);
        pageServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldUseParentPathWhenAddAsync()
    {
        authorizationBrokerMock
            .Setup(x => x.Authorize(It.IsAny<int?>(), It.IsAny<string>()))
            .Callback((int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId, privilege) ?? false))
                    throw new SecurityException("Access Denied!");
            });

        authorizationBrokerMock
            .Setup(x => x.IsAdminOfApp(It.IsAny<int>()))
            .Returns((int appId) => currentUser?.IsAdminOfApp(appId) ?? false);

        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(() => currentUser);

        User actor = TestUsers.WithPrivilege("app_admin", 1);
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
        pageServiceMock.Setup(x => x.AddPageAsync(It.IsAny<Page>())).ReturnsAsync(addedPage);
        pageServiceMock.Setup(x => x.GetAllPage(false)).Returns(new[] { parent }.AsQueryable());
        pageServiceMock.Setup(x => x.GetAllPage(true)).Returns(new[] { parent }.AsQueryable());

        Page result = await pageProcessingService.AddPageAsync(page);

        result.Should().BeSameAs(addedPage);
        pageServiceMock.Verify(x => x.GetAllPage(false), Times.Once);
        pageServiceMock.Verify(x => x.GetAllPage(true), Times.Once);
        pageServiceMock.Verify(
            x => x.AddPageAsync(It.Is<Page>(p => p.Path == "parent/Child" && p.AppId == 1)),
            Times.Once
        );
        pageServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowValidationExceptionWhenComputedPathAlreadyExistsForSameAppOnAddAsync()
    {
        authorizationBrokerMock
            .Setup(x => x.Authorize(It.IsAny<int?>(), It.IsAny<string>()))
            .Callback((int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId, privilege) ?? false))
                    throw new SecurityException("Access Denied!");
            });

        authorizationBrokerMock
            .Setup(x => x.IsAdminOfApp(It.IsAny<int>()))
            .Returns((int appId) => currentUser?.IsAdminOfApp(appId) ?? false);

        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(() => currentUser);

        currentUser = TestUsers.WithPrivilege("page_create", 1);
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

        pageServiceMock.Setup(x => x.GetAllPage(true)).Returns(new[] { existingPage }.AsQueryable());

        Func<Task> act = async () => await pageProcessingService.AddPageAsync(page);

        await act.Should()
            .ThrowAsync<System.ComponentModel.DataAnnotations.ValidationException>()
            .WithMessage("A page already exists for app 1 with path 'About'.");

        pageServiceMock.Verify(x => x.GetAllPage(true), Times.Once);
        pageServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldAllowComputedPathWhenItOnlyExistsForAnotherAppOnAddAsync()
    {
        authorizationBrokerMock
            .Setup(x => x.Authorize(It.IsAny<int?>(), It.IsAny<string>()))
            .Callback((int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId, privilege) ?? false))
                    throw new SecurityException("Access Denied!");
            });

        authorizationBrokerMock
            .Setup(x => x.IsAdminOfApp(It.IsAny<int>()))
            .Returns((int appId) => currentUser?.IsAdminOfApp(appId) ?? false);

        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(() => currentUser);

        currentUser = TestUsers.WithPrivilege("page_create", 1);
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

        pageServiceMock.Setup(x => x.GetAllPage(true)).Returns(new[] { existingPage }.AsQueryable());
        pageServiceMock.Setup(x => x.AddPageAsync(It.IsAny<Page>())).ReturnsAsync(addedPage);

        Page result = await pageProcessingService.AddPageAsync(page);

        result.Should().BeSameAs(addedPage);
        pageServiceMock.Verify(x => x.GetAllPage(true), Times.Once);
        pageServiceMock.Verify(
            x => x.AddPageAsync(It.Is<Page>(p => p.AppId == 1 && p.Path == "About")),
            Times.Once);
        pageServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowValidationExceptionWhenComputedPathExistsOnPageUserCannotSeeForSameAppOnAddAsync()
    {
        authorizationBrokerMock
            .Setup(x => x.Authorize(It.IsAny<int?>(), It.IsAny<string>()))
            .Callback((int? appId, string privilege) =>
            {
                if (!(currentUser?.Can(appId, privilege) ?? false))
                    throw new SecurityException("Access Denied!");
            });

        authorizationBrokerMock
            .Setup(x => x.IsAdminOfApp(It.IsAny<int>()))
            .Returns((int appId) => currentUser?.IsAdminOfApp(appId) ?? false);

        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(() => currentUser);

        currentUser = TestUsers.WithPrivilege("page_create", 1);
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

        pageServiceMock.Setup(x => x.GetAllPage(false)).Returns(new[] { parent }.AsQueryable());
        pageServiceMock.Setup(x => x.GetAllPage(true)).Returns(new[] { parent, hiddenDuplicate }.AsQueryable());

        Func<Task> act = async () => await pageProcessingService.AddPageAsync(page);

        await act.Should()
            .ThrowAsync<System.ComponentModel.DataAnnotations.ValidationException>()
            .WithMessage("A page already exists for app 1 with path 'parent/Child'.");

        pageServiceMock.Verify(x => x.GetAllPage(false), Times.Exactly(2));
        pageServiceMock.Verify(x => x.GetAllPage(true), Times.Once);
        pageServiceMock.VerifyNoOtherCalls();
    }
}










