// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using cCoder.Data.Models.CMS;

using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PageProcessingServiceTests
{
    [Fact]
    public void ShouldReturnDirectChildrenWhenGetChildren()
    {
        // Given



        Page parent = CreateRandomPage();
        Page child = CreateRandomPage();
        child.Id = 10;
        child.ParentId = parent.Id;

        pageServiceMock.Setup(expression: x => x.GetAllPages())
            .Returns(value: new[] { parent, child }.AsQueryable());

        // When
        Page[] result = pageProcessingService.GetChildrenPages(pageId: parent.Id)
            .ToArray();

        // Then
        result.Should()
            .ContainSingle();

        result[0].Id.Should()
            .Be(expected: child.Id);

        pageServiceMock.Verify(expression: x => x.GetAllPages(), times: Times.Once);
        pageServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldReturnEmptyCollectionWhenParentHasNoChildrenForGetChildren()
    {
        // Given



        Page parent = CreateRandomPage();
        Page other = CreateRandomPage();
        other.ParentId = parent.Id + 1;

        pageServiceMock.Setup(expression: x => x.GetAllPages())
            .Returns(value: new[] { parent, other }.AsQueryable());

        // When
        Page[] result = pageProcessingService.GetChildrenPages(pageId: parent.Id)
            .ToArray();

        // Then
        result.Should()
            .BeEmpty();

        pageServiceMock.Verify(expression: x => x.GetAllPages(), times: Times.Once);
        pageServiceMock.VerifyNoOtherCalls();
    }
}