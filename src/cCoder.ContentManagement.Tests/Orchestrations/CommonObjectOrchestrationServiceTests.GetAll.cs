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
    public void ShouldReturnProcessingResultsWhenGetAll()
    {
        // Given
        IQueryable<CommonObject> entities = new[] { CreateRandomCommonObject() }.AsQueryable();

        commonObjectProcessingServiceMock.Setup(expression: x => x.GetAllCommonObject(ignoreFilters: true))
            .Returns(value: entities);

        // When
        IQueryable<CommonObject> result = orchestrationService.GetAllCommonObject(ignoreFilters: true);

        // Then

        result.Should()
            .BeSameAs(expected: entities);

        commonObjectProcessingServiceMock.Verify(expression: x => x.GetAllCommonObject(ignoreFilters: true), times: Times.Once);
        commonObjectProcessingServiceMock.VerifyNoOtherCalls();
        commonObjectEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}