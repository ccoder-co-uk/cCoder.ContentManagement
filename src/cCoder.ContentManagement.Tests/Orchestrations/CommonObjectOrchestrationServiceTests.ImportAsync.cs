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
    public async Task ShouldNotRaiseImportedEventWhenNoObjectWasChangedAsync()
    {
        // Given
        CommonObject[] items = [CreateRandomCommonObject()];
        OperationResult<CommonObject>[] expectedResults = [];

        commonObjectProcessingServiceMock.Setup(expression: x => x.AddAllCommonObjectsAsync(newCommonObjects: items))
            .ReturnsAsync(value: expectedResults);

        // When
        IEnumerable<OperationResult<CommonObject>> result = await orchestrationService.AddAllCommonObjectsAsync(newCommonObjects: items);

        // Then
        result.Should()
            .BeSameAs(expected: expectedResults);

        commonObjectProcessingServiceMock.Verify(expression: x => x.AddAllCommonObjectsAsync(newCommonObjects: items), times: Times.Once);
        commonObjectProcessingServiceMock.VerifyNoOtherCalls();
        commonObjectEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}