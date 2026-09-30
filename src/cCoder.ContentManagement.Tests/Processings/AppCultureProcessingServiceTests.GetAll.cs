// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class AppCultureProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGetAll()
    {
        // Given
        IQueryable<AppCulture> entities = new[] { CreateRandomAppCulture() }.AsQueryable();

        appCultureServiceMock.Setup(expression: x => x.GetAllAppCultures())
            .Returns(value: entities);

        // When
        IQueryable<AppCulture> result = appCultureProcessingService.GetAllAppCultures();

        // Then

        result.Should()
            .BeSameAs(expected: entities);

        appCultureServiceMock.Verify(expression: x => x.GetAllAppCultures(), times: Times.Once);
        appCultureServiceMock.VerifyNoOtherCalls();
    }

}