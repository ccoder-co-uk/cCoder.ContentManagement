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
using FluentAssertions;
using Moq;
using Xunit;



namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class CommonObjectProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGet()
    {
        // Given
        CommonObject commonObject = CreateRandomCommonObject();

        commonObjectServiceMock.Setup(expression: x => x.GetCommonObject(commonObjectId: commonObject.Id))
            .Returns(value: commonObject);

        // When

        CommonObject result = commonObjectProcessingService.GetCommonObject(
commonObjectId: commonObject.Id
        );

        // Then

        result.Should()
            .BeSameAs(expected: commonObject);

        commonObjectServiceMock.Verify(expression: x => x.GetCommonObject(commonObjectId: commonObject.Id), times: Times.Once);
        commonObjectServiceMock.VerifyNoOtherCalls();
        commonObjectCacheMock.VerifyNoOtherCalls();
    }

}