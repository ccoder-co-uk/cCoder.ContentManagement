// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class TemplateProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGetAll()
    {
        // Given
        IQueryable<Template> entities = new[] { CreateRandomTemplate() }.AsQueryable();

        templateServiceMock.Setup(expression: x => x.GetAllTemplate())
            .Returns(value: entities);

        // When
        IQueryable<Template> result = templateProcessingService.GetAllTemplate();

        // Then

        result.Should()
            .BeSameAs(expected: entities);

        templateServiceMock.Verify(expression: x => x.GetAllTemplate(), times: Times.Once);
        templateServiceMock.VerifyNoOtherCalls();
    }

}