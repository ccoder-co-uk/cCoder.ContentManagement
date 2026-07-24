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

namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class CommonObjectOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnProcessingResultsWhenLatest()
    {
        // Given
        const string type = "TestType";
        CommonObject[] items = [CreateRandomCommonObject()];

        commonObjectProcessingServiceMock.Setup(expression: x => x.LatestCommonObject(type: type))
            .Returns(value: items);

        // When
        IEnumerable<CommonObject> result = orchestrationService.LatestCommonObject(type: type);

        // Then
        result.Should()
            .BeSameAs(expected: items);

        commonObjectProcessingServiceMock.Verify(expression: x => x.LatestCommonObject(type: type), times: Times.Once);
        commonObjectProcessingServiceMock.VerifyNoOtherCalls();
        commonObjectEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}