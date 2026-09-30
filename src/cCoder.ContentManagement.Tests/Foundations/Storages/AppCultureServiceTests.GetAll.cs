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

public partial class AppCultureServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGetAll()
    {
        // Given
        AppCulture[] expectedItems = [CreateRandomAppCulture()];

        IQueryable<CmsDataModels.AppCulture> appCultures = expectedItems
            .Select(selector: item => item)
            .AsQueryable();

        appCultureBrokerMock.Setup(expression: x => x.GetAllAppCultures())
            .Returns(value: appCultures);

        // When
        IQueryable<AppCulture> result = appCultureService.GetAllAppCultures();

        // Then

        result.Should()
            .BeEquivalentTo(expectation: expectedItems);

        appCultureBrokerMock.Verify(expression: x => x.GetAllAppCultures(), times: Times.Once);
        appCultureBrokerMock.VerifyNoOtherCalls();
    }

}