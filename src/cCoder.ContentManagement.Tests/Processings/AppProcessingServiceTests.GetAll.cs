// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class AppProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGetAll()
    {
        // Given
        IQueryable<App> entities = new[] { CreateRandomApp() }.AsQueryable();

        appServiceMock.Setup(expression: x => x.GetAllApp())
            .Returns(value: entities);

        // When
        IQueryable<App> result = appProcessingService.GetAllApp();

        // Then

        result.Should()
            .BeSameAs(expected: entities);

        appServiceMock.Verify(expression: x => x.GetAllApp(), times: Times.Once);
        appServiceMock.VerifyNoOtherCalls();
    }

}