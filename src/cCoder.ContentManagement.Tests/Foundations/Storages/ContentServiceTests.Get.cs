// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Storages;

public partial class ContentServiceTests
{
    [Fact]
    public void ShouldDelegateToBrokerWhenGet()
    {
        // Given
        Content content = CreateRandomContent(id: 7);

        contentBrokerMock.Setup(expression: x => x.GetAllContents())
            .Returns(value: new[] { content }.AsQueryable());

        // When
        Content result = contentService.GetContent(contentId: 7);

        // Then

        result.Should()
            .BeEquivalentTo(expectation: content);

        contentBrokerMock.Verify(expression: x => x.GetAllContents(), times: Times.Once);
        contentBrokerMock.VerifyNoOtherCalls();
    }

}