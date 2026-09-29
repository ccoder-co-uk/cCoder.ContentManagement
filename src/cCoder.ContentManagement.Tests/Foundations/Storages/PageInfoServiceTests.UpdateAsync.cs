// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;



using FluentAssertions;
using Moq;
using Xunit;
using DataPageInfo = cCoder.Data.Models.CMS.PageInfo;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class PageInfoServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUserIsAuthorizedForUpdateAsync()
    {
        // Given
        PageInfo pageInfo = CreateRandomPageInfo();

        DataPageInfo submitted = null;

        pageInfoBrokerMock
            .Setup(expression: x => x.UpdatePageInfoAsync(updatedPageInfo: It.IsAny<DataPageInfo>()))
            .Callback<DataPageInfo>(action: candidate => submitted = candidate)
            .ReturnsAsync(valueFunction: (DataPageInfo value) => value);

        // When
        PageInfo result = await pageInfoService.UpdatePageInfoAsync(updatedPageInfo: pageInfo);

        // Then

        result.Should()
            .BeSameAs(expected: pageInfo);

        submitted.Should()
            .NotBeNull();

        submitted.Should()
            .NotBeSameAs(unexpected: pageInfo);

        result.Should()
            .NotBeSameAs(unexpected: submitted);

        submitted.Should()
            .BeEquivalentTo(expectation: pageInfo);

        result.Should()
            .BeEquivalentTo(expectation: pageInfo);

        pageInfoBrokerMock.Verify(expression: x => x.UpdatePageInfoAsync(updatedPageInfo: It.IsAny<DataPageInfo>()), times: Times.Once);
        pageInfoBrokerMock.VerifyNoOtherCalls();
    }

}