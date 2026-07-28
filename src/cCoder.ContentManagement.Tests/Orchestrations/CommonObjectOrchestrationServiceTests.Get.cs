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



namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class CommonObjectOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnProcessingResultWhenGet()
    {
        // Given
        int id = 1;
        CommonObject entity = CreateRandomCommonObject();

        commonObjectProcessingServiceMock.Setup(expression: x => x.GetCommonObject(commonObjectId: id))
            .Returns(value: entity);

        // When
        CommonObject result = orchestrationService.GetCommonObject(commonObjectId: id);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        commonObjectProcessingServiceMock.Verify(expression: x => x.GetCommonObject(commonObjectId: id), times: Times.Once);
        commonObjectProcessingServiceMock.VerifyNoOtherCalls();
        commonObjectEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}