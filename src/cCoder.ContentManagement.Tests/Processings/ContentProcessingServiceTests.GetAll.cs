// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class ContentProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGetAll()
    {
        // Given
        IQueryable<Content> entities = new[] { CreateRandomContent() }.AsQueryable();

        contentServiceMock.Setup(expression: x => x.GetAllContents())
            .Returns(value: entities);

        // When
        IQueryable<Content> result = contentProcessingService.GetAllContents();

        // Then

        result.Should()
            .BeSameAs(expected: entities);

        contentServiceMock.Verify(expression: x => x.GetAllContents(), times: Times.Once);
        contentServiceMock.VerifyNoOtherCalls();
    }

}