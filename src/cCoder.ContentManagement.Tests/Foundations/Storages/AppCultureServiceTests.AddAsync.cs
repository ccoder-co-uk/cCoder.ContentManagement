// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;
using CmsDataModels = cCoder.Data.Models.CMS;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class AppCultureServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenAddAsync()
    {
        // Given
        AppCulture appCulture = CreateRandomAppCulture();

        CmsDataModels.AppCulture submitted = null;

        appCultureBrokerMock
            .Setup(expression: x =>
                x.AddAppCultureAsync(
newAppCulture: It.Is<CmsDataModels.AppCulture>(match: candidate => !ReferenceEquals(objA: candidate, objB: appCulture))
                )
            )
            .Callback<CmsDataModels.AppCulture>(action: candidate => submitted = candidate)
            .ReturnsAsync(valueFunction: (CmsDataModels.AppCulture value) => value);

        // When
        AppCulture result = await appCultureService.AddAppCultureAsync(newAppCulture: appCulture);

        // Then

        result.Should()
            .BeSameAs(expected: appCulture);

        submitted.Should()
            .NotBeNull();

        submitted.Should()
            .NotBeSameAs(unexpected: appCulture);

        result.Should()
            .NotBeSameAs(unexpected: submitted);

        submitted.Should()
            .BeEquivalentTo(expectation: appCulture);

        result.Should()
            .BeEquivalentTo(expectation: appCulture);

        appCultureBrokerMock.Verify(
expression: x =>
                x.AddAppCultureAsync(
newAppCulture: It.Is<CmsDataModels.AppCulture>(match: candidate => !ReferenceEquals(objA: candidate, objB: appCulture))
                ),
times: Times.Once
        );

        appCultureBrokerMock.VerifyNoOtherCalls();
    }

}