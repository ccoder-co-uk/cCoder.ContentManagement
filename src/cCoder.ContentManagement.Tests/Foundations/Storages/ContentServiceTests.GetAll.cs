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

public partial class ContentServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGetAll()
    {
        // Given
        Content[] expectedItems = [CreateRandomContent()];

        IQueryable<CmsDataModels.Content> contents = expectedItems
            .Select(selector: item => item)
            .AsQueryable();

        contentBrokerMock.Setup(expression: x => x.GetAllContents())
            .Returns(value: contents);

        // When
        IQueryable<Content> result = contentService.GetAllContent();

        // Then

        result.Should()
            .BeEquivalentTo(expectation: expectedItems);

        contentBrokerMock.Verify(expression: x => x.GetAllContents(), times: Times.Once);
        contentBrokerMock.VerifyNoOtherCalls();
    }

}