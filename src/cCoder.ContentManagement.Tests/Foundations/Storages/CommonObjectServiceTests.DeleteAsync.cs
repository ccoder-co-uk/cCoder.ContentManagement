// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.Security;
using ComponentRenderParams = cCoder.ContentManagement.Models.ComponentRenderParams;
using Config = cCoder.ContentManagement.Models.ContentManagementConfiguration;
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
        DataCommonObject dataCommonObject = ToDataCommonObject(commonObject: commonObject);

        commonObjectBrokerMock.Setup(expression: x => x.GetAllCommonObjects(ignoreFilters: false))
            .Returns(value: new[] { dataCommonObject }.AsQueryable());

        commonObjectBrokerMock.Setup(expression: x => x.GetAppId(entity: It.IsAny<DataCommonObject>()))
            .Returns(value: (int?)7);

        authorizationBrokerMock.Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "CommonObject_delete"));

        commonObjectBrokerMock
            .Setup(expression: x => x.DeleteCommonObjectAsync(deletedCommonObject: It.Is<DataCommonObject>(match: candidate => candidate.Id == commonObject.Id)))
            .ReturnsAsync(value: 1);

        // When
        await commonObjectService.DeleteAsync(commonObjectId: 9);

        // Then
        commonObjectBrokerMock.Verify(expression: x => x.GetAllCommonObjects(ignoreFilters: false), times: Times.Once);

        commonObjectBrokerMock.Verify(
expression: x => x.DeleteCommonObjectAsync(deletedCommonObject: It.Is<DataCommonObject>(match: candidate => candidate.Id == commonObject.Id)),
times: Times.Once
        );

        commonObjectBrokerMock.Verify(
expression: x => x.GetAppId(entity: It.IsAny<DataCommonObject>()),
times: Times.AtMostOnce()
        );

        commonObjectBrokerMock.VerifyNoOtherCalls();

        authorizationBrokerMock.Verify(
expression: x => x.Authorize(appId: (int?)7, privilege: "CommonObject_delete"),
times: Times.Once
        );

        authorizationBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUserLacksDeletePrivilegeForDeleteAsync()
    {
        // Given
        CommonObject commonObject = CreateRandomCommonObject(id: 9);

        commonObjectBrokerMock
            .Setup(expression: x => x.GetAllCommonObjects(ignoreFilters: false))
            .Returns(value: new[] { ToDataCommonObject(commonObject: commonObject) }.AsQueryable());

        commonObjectBrokerMock.Setup(expression: x => x.GetAppId(entity: It.IsAny<DataCommonObject>()))
            .Returns(value: (int?)7);

        authorizationBrokerMock
            .Setup(expression: x => x.Authorize(appId: (int?)7, privilege: "CommonObject_delete"))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () => await commonObjectService.DeleteAsync(commonObjectId: 9);

        // Then

        await action.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        commonObjectBrokerMock.Verify(expression: x => x.GetAllCommonObjects(ignoreFilters: false), times: Times.Once);

        commonObjectBrokerMock.Verify(
expression: x => x.GetAppId(entity: It.IsAny<DataCommonObject>()),
times: Times.AtMostOnce()
        );

        commonObjectBrokerMock.VerifyNoOtherCalls();

        authorizationBrokerMock.Verify(
expression: x => x.Authorize(appId: (int?)7, privilege: "CommonObject_delete"),
times: Times.Once
        );

        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}