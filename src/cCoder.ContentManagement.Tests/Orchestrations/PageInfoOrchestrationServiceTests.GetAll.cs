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

public partial class PageInfoOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnProcessingResultsWhenGetAll()
    {
        // Given
        IQueryable<PageInfo> entities = new[] { CreateRandomPageInfo() }.AsQueryable();

        pageInfoProcessingServiceMock.Setup(expression: x => x.GetAllPageInfo(ignoreFilters: true))
            .Returns(value: entities);

        // When
        IQueryable<PageInfo> result = orchestrationService.GetAllPageInfo(ignoreFilters: true);

        // Then

        result.Should()
            .BeSameAs(expected: entities);

        pageInfoProcessingServiceMock.Verify(expression: x => x.GetAllPageInfo(ignoreFilters: true), times: Times.Once);
        pageInfoProcessingServiceMock.VerifyNoOtherCalls();
        pageInfoEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}