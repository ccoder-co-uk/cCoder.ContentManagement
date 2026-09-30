// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class CultureProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGetAll()
    {
        // Given
        IQueryable<Culture> entities = new[] { CreateRandomCulture() }.AsQueryable();

        cultureServiceMock.Setup(expression: x => x.GetAllCultures())
            .Returns(value: entities);

        // When
        IQueryable<Culture> result = cultureProcessingService.GetAllCultures();

        // Then

        result.Should()
            .BeSameAs(expected: entities);

        cultureServiceMock.Verify(expression: x => x.GetAllCultures(), times: Times.Once);
        cultureServiceMock.VerifyNoOtherCalls();
    }

}