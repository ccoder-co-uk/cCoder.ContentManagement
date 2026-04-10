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

public partial class ContentServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForDeleteAsync()
    {
        // Given
        Content content = CreateRandomContent(id: 9);

        contentBrokerMock.Setup(x => x.GetAllContents(false)).Returns(new[] { content }.AsQueryable());

        pageBrokerMock.Setup(x => x.GetAllPages(true)).Returns(new[] { new CmsDataModels.Page { Id = content.PageId, AppId = 7 } }.AsQueryable());
        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "Content_delete"));
        contentBrokerMock
            .Setup(
                x =>
                    x.DeleteContentAsync(
                        It.Is<CmsDataModels.Content>(item => item.Id == content.Id)
                    )
            )
            .ReturnsAsync(1);

        // When
        await contentService.DeleteAsync(9);

        // Then
        contentBrokerMock.Verify(x => x.GetAllContents(false), Times.Once);
        contentBrokerMock.Verify(
            x => x.DeleteContentAsync(It.Is<CmsDataModels.Content>(item => item.Id == content.Id)),
            Times.Once
        );
        contentBrokerMock.VerifyNoOtherCalls();
        pageBrokerMock.Verify(x => x.GetAllPages(true), Times.Once);
        pageBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "Content_delete"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        Content content = CreateRandomContent(id: 9);

        contentBrokerMock.Setup(x => x.GetAllContents(false)).Returns(new[] { content }.AsQueryable());

        pageBrokerMock.Setup(x => x.GetAllPages(true)).Returns(new[] { new CmsDataModels.Page { Id = content.PageId, AppId = 7 } }.AsQueryable());
        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "Content_delete"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await contentService.DeleteAsync(9);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        contentBrokerMock.Verify(x => x.GetAllContents(false), Times.Once);
        contentBrokerMock.VerifyNoOtherCalls();
        pageBrokerMock.Verify(x => x.GetAllPages(true), Times.Once);
        pageBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "Content_delete"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}















