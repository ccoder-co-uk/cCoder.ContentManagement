// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;
using CmsDataModels = cCoder.Data.Models.CMS;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class CultureServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGetAll()
    {
        // Given
        Culture[] expectedItems = [CreateRandomCulture()];

        IQueryable<CmsDataModels.Culture> cultures = expectedItems
            .Select(selector: item => item)
            .AsQueryable();

        cultureBrokerMock.Setup(expression: x => x.GetAllCultures())
            .Returns(value: cultures);

        // When
        IQueryable<Culture> result = cultureService.GetAllCultures();

        // Then

        result.Should()
            .BeEquivalentTo(expectation: expectedItems);

        cultureBrokerMock.Verify(expression: x => x.GetAllCultures(), times: Times.Once);
        cultureBrokerMock.VerifyNoOtherCalls();
    }

}