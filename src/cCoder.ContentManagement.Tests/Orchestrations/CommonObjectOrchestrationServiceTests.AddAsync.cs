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
    public async Task ShouldAddAllThenRaiseImportedEventAsync()
    {
        // Given
        CommonObject entity = CreateRandomCommonObject();
        CommonObject[] commonObjects = [entity];

        OperationResult<CommonObject>[] expectedResults =
        [
            new OperationResult<CommonObject>
            {
                Success = true,
                Item = entity
            }
        ];

        commonObjectProcessingServiceMock
            .Setup(expression: x => x.AddAllCommonObjectsAsync(
                newCommonObjects: commonObjects))
            .ReturnsAsync(value: expectedResults);

        commonObjectEventProcessingServiceMock
            .Setup(expression: x => x.RaiseCommonObjectsImportedEventAsync(
                commonObjects: It.Is<CommonObject[]>(
                    match: objects => objects.SequenceEqual(second: commonObjects))))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await orchestrationService.AddAllCommonObjectsAsync(
            newCommonObjects: commonObjects);

        // Then
        commonObjectEventProcessingServiceMock.VerifyAll();
    }
}