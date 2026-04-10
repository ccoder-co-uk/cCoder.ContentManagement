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
    public void ShouldRenderChildMenuItemsWhenMenuFor()
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

        Page child = CreateRandomPage();
        child.ParentId = 10;
        child.Path = "docs";
        child.ShowOnMenus = true;
        child.Order = 1;
        child.PageInfo =
        [
            new PageInfo
            {
                CultureId = string.Empty,
                Title = "Docs",
                PageId = child.Id,
                Page = null!,
                Culture = null!,
            },
        ];
        pageServiceMock.Setup(x => x.GetAll(false)).Returns(new[] { child }.AsQueryable());

        string result = pageProcessingService.MenuFor(10, string.Empty);

        result.Should().Contain("<ul class='submenu'>");
        result.Should().Contain("/docs");
        result.Should().Contain("Docs");
        pageServiceMock.Verify(x => x.GetAll(false), Times.Once);
        pageServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldRenderEmptySubmenuWhenNoVisibleChildrenExistForMenuFor()
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

        pageServiceMock.Setup(x => x.GetAll(false)).Returns(Array.Empty<Page>().AsQueryable());

        string result = pageProcessingService.MenuFor(10, string.Empty);

        result.Should().Be("<ul class='submenu'></ul>");
        pageServiceMock.Verify(x => x.GetAll(false), Times.Once);
        pageServiceMock.VerifyNoOtherCalls();
    }
}










