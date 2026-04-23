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
using SecurityDataModels = cCoder.Data.Models.Security;
namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class CommonObjectServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForUpdateAsync()
    {
        // Given
        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(new SecurityDataModels.User { Id = "test-user" });
        CommonObject commonObject = CreateRandomCommonObject(id: 7);

        DataCommonObject submitted = null;

        commonObjectBrokerMock.Setup(x => x.GetAppId(It.IsAny<DataCommonObject>())).Returns((int?)7);
        authorizationBrokerMock.Setup(x => x.Authorize((int?)7, "CommonObject_update"));

        commonObjectBrokerMock
            .Setup(x => x.UpdateCommonObjectAsync(It.IsAny<DataCommonObject>()))
            .Callback<DataCommonObject>(candidate => submitted = candidate)
            .ReturnsAsync((DataCommonObject value) => value);

        // When
        CommonObject result = await commonObjectService.UpdateAsync(commonObject);

        // Then
        result.Should().BeSameAs(commonObject);
        submitted.Should().NotBeNull();
        submitted.Should().NotBeSameAs(commonObject);
        result.Should().NotBeSameAs(submitted);

        submitted
            .Should()
            .BeEquivalentTo(
                commonObject,
                options =>
                    options
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("CreatedOn")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("CreatedBy")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("LastUpdated")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("LastUpdatedBy")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("LastUpdatedOn")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("UpdatedBy")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("Created")
                        )
            );

        result
            .Should()
            .BeEquivalentTo(
                commonObject,
                options =>
                    options
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("CreatedOn")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("CreatedBy")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("LastUpdated")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("LastUpdatedBy")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("LastUpdatedOn")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("UpdatedBy")
                        )
                        .Excluding(
                            (FluentAssertions.Equivalency.IMemberInfo info) =>
                                info.Path.EndsWith("Created")
                        )
            );

        commonObjectBrokerMock.Verify(
            x => x.UpdateCommonObjectAsync(It.IsAny<DataCommonObject>()),
            Times.Once
        );
        commonObjectBrokerMock.Verify(
            x => x.GetAppId(It.IsAny<DataCommonObject>()),
            Times.AtMostOnce()
        );
        commonObjectBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(
            x => x.Authorize((int?)7, "CommonObject_update"),
            Times.Once
        );
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksUpdatePrivilegeForUpdateAsync()
    {
        // Given
        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(new SecurityDataModels.User { Id = "test-user" });
        CommonObject commonObject = CreateRandomCommonObject(id: 7);

        commonObjectBrokerMock.Setup(x => x.GetAppId(It.IsAny<DataCommonObject>())).Returns((int?)7);
        authorizationBrokerMock
            .Setup(x => x.Authorize((int?)7, "CommonObject_update"))
            .Throws(new SecurityException("Access Denied!"));

        // When
        Func<Task> action = async () => await commonObjectService.UpdateAsync(commonObject);

        // Then
        await action.Should().ThrowAsync<SecurityException>().WithMessage("Access Denied!");
        commonObjectBrokerMock.Verify(
            x => x.GetAppId(It.IsAny<DataCommonObject>()),
            Times.AtMostOnce()
        );
        commonObjectBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.Verify(
            x => x.Authorize((int?)7, "CommonObject_update"),
            Times.Once
        );
    }

}




















