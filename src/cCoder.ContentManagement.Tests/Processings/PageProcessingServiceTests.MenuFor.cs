// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Linq;
using cCoder.Data.Models.CMS;

using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PageProcessingServiceTests
{
    [Fact]
    public void ShouldRenderChildMenuItemsWhenMenuFor()
    {
        // Given



        Page child = CreateRandomPage();
        child.ParentId = 10;
        child.Path = "docs";
        child.ShowOnMenus = true;
        child.Order = 1;

        child.PageInfo =
        [
            new PageInfo
            {
                CultureId = string.Empty,
                Title = "Docs",
                PageId = child.Id,
                Page = null!,
                Culture = null!,
            },
        ];

        pageServiceMock.Setup(expression: x => x.GetAllPages())
            .Returns(value: new[] { child }.AsQueryable());

        // When
        string result = pageProcessingService.MenuFor(pageId: 10, culture: string.Empty);

        // Then
        result.Should()
            .Contain(expected: "<ul class='submenu'>");

        result.Should()
            .Contain(expected: "/docs");

        result.Should()
            .Contain(expected: "Docs");

        pageServiceMock.Verify(expression: x => x.GetAllPages(), times: Times.Once);
        pageServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldRenderEmptySubmenuWhenNoVisibleChildrenExistForMenuFor()
    {
        // Given



        pageServiceMock.Setup(expression: x => x.GetAllPages())
            .Returns(value: Array.Empty<Page>()
            .AsQueryable());

        // When
        string result = pageProcessingService.MenuFor(pageId: 10, culture: string.Empty);

        // Then
        result.Should()
            .Be(expected: "<ul class='submenu'></ul>");

        pageServiceMock.Verify(expression: x => x.GetAllPages(), times: Times.Once);
        pageServiceMock.VerifyNoOtherCalls();
    }
}