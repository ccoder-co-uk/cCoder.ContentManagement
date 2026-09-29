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

public partial class ContentServiceTests
{
    [Fact]
    public async Task ShouldDelegateToBrokerWhenUpdateAsync()
    {
        // Given
        Content content = CreateRandomContent(id: 7);

        CmsDataModels.Content submitted = null;

        contentBrokerMock
            .Setup(expression: x => x.UpdateContentAsync(updatedContent: It.IsAny<CmsDataModels.Content>()))
            .Callback<CmsDataModels.Content>(action: candidate => submitted = candidate)
            .ReturnsAsync(valueFunction: (CmsDataModels.Content value) => value);

        // When
        Content result = await contentService.UpdateContentAsync(updatedContent: content);

        // Then

        result.Should()
            .BeSameAs(expected: content);

        submitted.Should()
            .NotBeNull();

        submitted.Should()
            .NotBeSameAs(unexpected: content);

        result.Should()
            .NotBeSameAs(unexpected: submitted);

        submitted.Should()
            .BeEquivalentTo(expectation: content);

        result.Should()
            .BeEquivalentTo(expectation: content);

        contentBrokerMock.Verify(expression: x => x.UpdateContentAsync(updatedContent: It.IsAny<CmsDataModels.Content>()), times: Times.Once);
        contentBrokerMock.VerifyNoOtherCalls();
    }

}