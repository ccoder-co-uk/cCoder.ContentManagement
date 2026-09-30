// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class TemplateOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnProcessingResultsWhenGetAll()
    {
        // Given
        IQueryable<Template> entities = new[] { CreateRandomTemplate() }.AsQueryable();

        templateProcessingServiceMock.Setup(expression: x => x.GetAllTemplates(ignoreFilters: true))
            .Returns(value: entities);

        // When

        var result = orchestrationService.GetAllTemplates(ignoreFilters: true)
            .ToArray();

        // Then

        result.Select(selector: item => item.Id)
            .Should()
            .Equal(expected: entities.Select(selector: item => item.Id));

        templateProcessingServiceMock.Verify(expression: x => x.GetAllTemplates(ignoreFilters: true), times: Times.Once);
        templateProcessingServiceMock.VerifyNoOtherCalls();
        templateEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}