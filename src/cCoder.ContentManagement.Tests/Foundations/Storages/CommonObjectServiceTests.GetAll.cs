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
using FluentAssertions;
using Moq;
using Xunit;

using DataCommonObject = cCoder.Data.Models.CommonObject;
namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class CommonObjectServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGetAll()
    {
        // Given
        CommonObject commonObject = CreateRandomCommonObject();
        IQueryable<DataCommonObject> commonObjects = new[] { ToDataCommonObject(commonObject) }.AsQueryable();

        commonObjectBrokerMock.Setup(x => x.GetAllCommonObjects(false)).Returns(commonObjects);

        // When
        IQueryable<CommonObject> result = commonObjectService.GetAll();

        // Then
        result.Should().BeEquivalentTo([commonObject]);
        commonObjectBrokerMock.Verify(x => x.GetAllCommonObjects(false), Times.Once);
        commonObjectBrokerMock.Verify(
            x => x.GetAppId(It.IsAny<DataCommonObject>()),
            Times.AtMostOnce()
        );
        commonObjectBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}


















