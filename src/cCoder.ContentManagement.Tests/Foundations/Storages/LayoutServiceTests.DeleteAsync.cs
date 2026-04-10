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

public partial class LayoutServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForDeleteAsync()
    {
        // Given
        Layout layout = CreateRandomLayout(id: 9, appId: 7);

        layoutBrokerMock.Setup(x => x.GetAllLayouts(false)).Returns(new[] { layout }.AsQueryable());

        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "Layout_delete"));
        layoutBrokerMock.Setup(x => x.DeleteLayoutAsync(It.IsAny<CmsDataModels.Layout>())).ReturnsAsync(1);

        // When
        await layoutService.DeleteAsync(9);

        // Then
        layoutBrokerMock.Verify(x => x.GetAllLayouts(false), Times.Once);
        layoutBrokerMock.Verify(x => x.DeleteLayoutAsync(It.Is<CmsDataModels.Layout>(actual => actual.Id == layout.Id)), Times.Once);
        layoutBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "Layout_delete"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        Layout layout = CreateRandomLayout(id: 9, appId: 7);

        layoutBrokerMock.Setup(x => x.GetAllLayouts(false)).Returns(new[] { layout }.AsQueryable());

        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "Layout_delete"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await layoutService.DeleteAsync(9);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        layoutBrokerMock.Verify(x => x.GetAllLayouts(false), Times.Once);
        layoutBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(x => x.Authorize((int?)7, "Layout_delete"), Times.Once);
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}















