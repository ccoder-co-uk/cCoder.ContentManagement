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
    public async Task ShouldRecomputePathsAndSaveWhenUserIsAppAdminForRecomputeAllForAppAsync()
    {
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
        Page page = CreateRandomPage(actor);
        page.Name = "Home";
        page.Path = "home";
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

        currentUser = actor;
        pageServiceMock.Setup(x => x.GetAll(true)).Returns(new[] { page }.AsQueryable());
        pageServiceMock
            .Setup(x => x.UpdateAsync(It.Is<Page>(updated => updated.Id == page.Id && updated.Path == string.Empty)))
            .Callback<Page>(updated => page.Path = updated.Path)
            .ReturnsAsync(page);

        // When
        await pageProcessingService.RecomputeAllForAppAsync(1);

        // Then
        page.Path.Should().Be(string.Empty);
        pageServiceMock.Verify(x => x.GetAll(true), Times.Once);
        pageServiceMock.Verify(x => x.UpdateAsync(It.Is<Page>(updated => updated.Id == page.Id && updated.Path == string.Empty)), Times.Once);
        pageServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserIsNotAppAdminForRecomputeAllForAppAsync()
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

        User actor = TestUsers.WithoutPrivileges();

        currentUser = actor;

        // When
        Func<Task> act = async () => await pageProcessingService.RecomputeAllForAppAsync(1);

        // Then
        await act.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        pageServiceMock.VerifyNoOtherCalls();
    }

}
















