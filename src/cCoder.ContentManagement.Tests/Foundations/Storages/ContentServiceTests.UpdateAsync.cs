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
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForUpdateAsync()
    {
        // Given
        Content content = CreateRandomContent(id: 7);

        CmsDataModels.Content submitted = null;

        pageBrokerMock.Setup(x => x.GetAllPages(true)).Returns(new[] { new CmsDataModels.Page { Id = content.PageId, AppId = 7 } }.AsQueryable());
        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "Content_update"));

        contentBrokerMock
            .Setup(x => x.UpdateContentAsync(It.IsAny<CmsDataModels.Content>()))
            .Callback<CmsDataModels.Content>(candidate => submitted = candidate)
            .ReturnsAsync((CmsDataModels.Content value) => value);

        // When
        Content result = await contentService.UpdateAsync(content);

        // Then
        result.Should().BeSameAs(content);
        submitted.Should().NotBeNull();
        submitted.Should().NotBeSameAs(content);
        result.Should().NotBeSameAs(submitted);
        submitted.Should().BeEquivalentTo(content);
        result.Should().BeEquivalentTo(content);
        contentBrokerMock.Verify(x => x.UpdateContentAsync(It.IsAny<CmsDataModels.Content>()), Times.Once);
        contentBrokerMock.VerifyNoOtherCalls();
        pageBrokerMock.Verify(x => x.GetAllPages(true), Times.Once);
        pageBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "Content_update"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksUpdatePrivilegeForUpdateAsync()
    {
        // Given
        Content content = CreateRandomContent(id: 7);

        pageBrokerMock.Setup(x => x.GetAllPages(true)).Returns(new[] { new CmsDataModels.Page { Id = content.PageId, AppId = 7 } }.AsQueryable());
        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "Content_update"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await contentService.UpdateAsync(content);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        contentBrokerMock.VerifyNoOtherCalls();
        pageBrokerMock.Verify(x => x.GetAllPages(true), Times.Once);
        pageBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "Content_update"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}















