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

public partial class PageServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForDeleteAsync()
    {
        // Given
        Page page = CreateRandomPage(id: 5);

        pageBrokerMock.Setup(x => x.GetAllPages(false)).Returns(new[] { page }.AsQueryable());
        authorizationBrokerMock.Setup(x => x.Authorize((int?)page.AppId, "Page_delete"));
        pageBrokerMock
            .Setup(x => x.DeletePageAsync(It.Is<CmsDataModels.Page>(p => p.Id == page.Id)))
            .ReturnsAsync(1);

        // When
        await pageService.DeleteAsync(5);

        // Then
        pageBrokerMock.Verify(x => x.GetAllPages(false), Times.Once);
        pageBrokerMock.Verify(x => x.DeletePageAsync(It.Is<CmsDataModels.Page>(p => p.Id == page.Id)), Times.Once);
        pageBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)page.AppId, "Page_delete"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        Page page = CreateRandomPage(id: 5);

        pageBrokerMock.Setup(x => x.GetAllPages(false)).Returns(new[] { page }.AsQueryable());

        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)page.AppId, "Page_delete"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await pageService.DeleteAsync(5);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        pageBrokerMock.Verify(x => x.GetAllPages(false), Times.Once);
        pageBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)page.AppId, "Page_delete"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}















