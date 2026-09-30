// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class ResourceProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGetAll()
    {
        // Given
        Resource[] resources = [CreateRandomResource()];
        IQueryable<Resource> queryableResources = resources.AsQueryable();

        resourceServiceMock.Setup(expression: x => x.GetAllResources())
            .Returns(value: queryableResources);

        // When
        IQueryable<Resource> result = resourceProcessingService.GetAllResources();

        // Then

        result.Should()
            .BeSameAs(expected: queryableResources);

        resourceServiceMock.Verify(expression: x => x.GetAllResources(), times: Times.Once);
        resourceServiceMock.VerifyNoOtherCalls();
    }

}