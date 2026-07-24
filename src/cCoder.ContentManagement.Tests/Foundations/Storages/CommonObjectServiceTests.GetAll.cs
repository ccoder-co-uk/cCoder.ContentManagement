// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        IQueryable<DataCommonObject> commonObjects = new[] { ToDataCommonObject(commonObject: commonObject) }.AsQueryable();

        commonObjectBrokerMock.Setup(expression: x => x.GetAllCommonObjects(ignoreFilters: false))
            .Returns(value: commonObjects);

        // When
        IQueryable<CommonObject> result = commonObjectService.GetAllCommonObject();

        // Then

        result.Should()
            .BeEquivalentTo(expectation: [commonObject]);

        commonObjectBrokerMock.Verify(expression: x => x.GetAllCommonObjects(ignoreFilters: false), times: Times.Once);

        commonObjectBrokerMock.Verify(
expression: x => x.GetAppId(entity: It.IsAny<DataCommonObject>()),
times: Times.AtMostOnce()
        );

        commonObjectBrokerMock.VerifyNoOtherCalls();
        authorizationBrokerMock.VerifyNoOtherCalls();
    }

}