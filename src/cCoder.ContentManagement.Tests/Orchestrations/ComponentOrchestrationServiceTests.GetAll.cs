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

public partial class ComponentOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnProcessingResultsWhenGetAll()
    {
        // Given
        IQueryable<Component> entities = new[] { CreateRandomComponent() }.AsQueryable();

        componentProcessingServiceMock.Setup(expression: x => x.GetAllComponent(ignoreFilters: true))
            .Returns(value: entities);

        // When

        var result = orchestrationService.GetAllComponent(ignoreFilters: true)
            .ToArray();

        // Then

        result.Select(selector: item => item.Id)
            .Should()
            .Equal(expected: entities.Select(selector: item => item.Id));

        componentProcessingServiceMock.Verify(expression: x => x.GetAllComponent(ignoreFilters: true), times: Times.Once);
        componentProcessingServiceMock.VerifyNoOtherCalls();
        componentEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}