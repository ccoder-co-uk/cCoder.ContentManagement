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

using DataCommonObject = cCoder.Data.Models.CommonObject;
namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class CommonObjectServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForDeleteAsync()
    {
        // Given
        CommonObject commonObject = CreateRandomCommonObject(id: 9);
        DataCommonObject dataCommonObject = ToDataCommonObject(commonObject);

        commonObjectBrokerMock.Setup(x => x.GetAllCommonObjects(false)).Returns(new[] { dataCommonObject }.AsQueryable());

        commonObjectBrokerMock.Setup(x => x.GetAppId(It.IsAny<DataCommonObject>())).Returns((int?)7);
        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "CommonObject_delete"));
        commonObjectBrokerMock
            .Setup(x => x.DeleteCommonObjectAsync(It.Is<DataCommonObject>(candidate => candidate.Id == commonObject.Id)))
            .ReturnsAsync(1);

        // When
        await commonObjectService.DeleteAsync(9);

        // Then
        commonObjectBrokerMock.Verify(x => x.GetAllCommonObjects(false), Times.Once);
        commonObjectBrokerMock.Verify(
            x => x.DeleteCommonObjectAsync(It.Is<DataCommonObject>(candidate => candidate.Id == commonObject.Id)),
            Times.Once
        );
        commonObjectBrokerMock.Verify(
            x => x.GetAppId(It.IsAny<DataCommonObject>()),
            Times.AtMostOnce()
        );
        commonObjectBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(
            x => x.Authorize((int?)7, "CommonObject_delete"),
            Times.Once
        );
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        CommonObject commonObject = CreateRandomCommonObject(id: 9);

        commonObjectBrokerMock
            .Setup(x => x.GetAllCommonObjects(false))
            .Returns(new[] { ToDataCommonObject(commonObject) }.AsQueryable());

        commonObjectBrokerMock.Setup(x => x.GetAppId(It.IsAny<DataCommonObject>())).Returns((int?)7);
        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "CommonObject_delete"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await commonObjectService.DeleteAsync(9);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        commonObjectBrokerMock.Verify(x => x.GetAllCommonObjects(false), Times.Once);
        commonObjectBrokerMock.Verify(
            x => x.GetAppId(It.IsAny<DataCommonObject>()),
            Times.AtMostOnce()
        );
        commonObjectBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(
            x => x.Authorize((int?)7, "CommonObject_delete"),
            Times.Once
        );
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}




















