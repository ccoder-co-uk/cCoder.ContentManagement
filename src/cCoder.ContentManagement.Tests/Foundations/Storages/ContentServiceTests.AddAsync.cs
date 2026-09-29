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
    public async Task ShouldDelegateToBrokerWhenAddAsync()
    {
        // Given
        Content content = CreateRandomContent(id: 0);

        CmsDataModels.Content submitted = null;

        contentBrokerMock
            .Setup(expression: x =>
                x.AddContentAsync(newContent: It.Is<CmsDataModels.Content>(match: candidate => !ReferenceEquals(objA: candidate, objB: content)))
            )
            .Callback<CmsDataModels.Content>(action: candidate => submitted = candidate)
            .ReturnsAsync(valueFunction: (CmsDataModels.Content value) => value);

        // When
        Content result = await contentService.AddContentAsync(newContent: content);

        // Then

        result.Should()
            .BeSameAs(expected: content);

        submitted.Should()
            .NotBeNull();

        submitted.Should()
            .NotBeSameAs(unexpected: content);

        result.Should()
            .NotBeSameAs(unexpected: submitted);

        submitted
            .Should()
            .BeEquivalentTo(expectation: content, config: options => options.Excluding(expression: candidate => candidate.Id));

        result
            .Should()
            .BeEquivalentTo(expectation: content, config: options => options.Excluding(expression: candidate => candidate.Id));

        contentBrokerMock.Verify(
expression: x =>
                x.AddContentAsync(
newContent: It.Is<CmsDataModels.Content>(match: candidate => !ReferenceEquals(objA: candidate, objB: content))
                ),
times: Times.Once
        );

        contentBrokerMock.VerifyNoOtherCalls();
    }

}