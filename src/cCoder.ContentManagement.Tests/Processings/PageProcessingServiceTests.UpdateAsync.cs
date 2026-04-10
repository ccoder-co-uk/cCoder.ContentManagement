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
    public async Task ShouldUpdatePageWhenUserCanUpdatePageForUpdateAsync()
    {
        // When

        // Then
        // Given
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
        pageServiceMock.Setup(x => x.GetAll(true))
            .Returns(new[] { dbPage }.AsQueryable());
        pageServiceMock.Setup(x => x.GetAll(false)).Returns(new[] { dbPage }.AsQueryable());
        pageServiceMock.Setup(x => x.UpdateAsync(It.IsAny<Page>())).ReturnsAsync(dbPage);
        // When
        Page result = await pageProcessingService.UpdateAsync(page);

        // Then
        result.Should().BeSameAs(dbPage);
        pageServiceMock.Verify(x => x.GetAll(false), Times.Once);
        pageServiceMock.Verify(x => x.GetAll(true), Times.Once);
        pageServiceMock.Verify(x => x.UpdateAsync(It.Is<Page>(updated =>
            updated.Id == page.Id &&
            updated.AppId == page.AppId &&
            updated.Name == page.Name)), Times.Once);
        pageServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserCannotUpdatePageForUpdateAsync()
    {
        // Given
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

        Page page = CreateRandomPage();
        pageServiceMock.Setup(x => x.GetAll(true)).Returns(new[] { page }.AsQueryable());
        pageServiceMock.Setup(x => x.GetAll(false)).Returns(new[] { page }.AsQueryable());

        // When
        Func<Task> act = async () => await pageProcessingService.UpdateAsync(page);

        // Then
        await act.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        pageServiceMock.Verify(x => x.GetAll(false), Times.Once);
        pageServiceMock.Verify(x => x.GetAll(true), Times.Once);
        pageServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenNewParentCannotBeResolvedForUpdateAsync()
    {
        // When

        // Then
        // Given
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
        pageServiceMock.SetupSequence(x => x.GetAll(true))
            .Returns(new[] { dbPage }.AsQueryable())
            .Returns(Array.Empty<Page>().AsQueryable());
        pageServiceMock.Setup(x => x.GetAll(false)).Returns(new[] { dbPage }.AsQueryable());

        // When
        Func<Task> act = async () => await pageProcessingService.UpdateAsync(page);

        // Then
        await act.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        pageServiceMock.Verify(x => x.GetAll(false), Times.Once);
        pageServiceMock.Verify(x => x.GetAll(true), Times.Exactly(2));
        pageServiceMock.VerifyNoOtherCalls();
    }

}
















