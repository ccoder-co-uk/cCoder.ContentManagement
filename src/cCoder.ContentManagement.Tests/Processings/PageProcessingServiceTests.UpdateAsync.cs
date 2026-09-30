// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using System.Security;

using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class PageProcessingServiceTests
{
    [Fact]
    public async Task UpdatePage_WhenOrderMovesEarlier_ShouldNormalizeSiblingOrders()
    {
        // Given
        Page firstPage = CreateRandomPage();
        firstPage.Order = 0;

        Page secondPage = CreateRandomPage();
        secondPage.Order = 1;

        Page movedPage = CreateRandomPage();
        movedPage.Order = 2;

        Page updatedPage = CreateRandomPage();
        updatedPage.Id = movedPage.Id;
        updatedPage.AppId = movedPage.AppId;
        updatedPage.ParentId = movedPage.ParentId;
        updatedPage.Order = 0;

        IQueryable<Page> storedPages = new[]
        {
            firstPage,
            secondPage,
            movedPage
        }.AsQueryable();

        pageServiceMock.Setup(expression: service =>
                service.GetAllPages(ignoreFilters: true))
            .Returns(value: storedPages);

        pageServiceMock.Setup(expression: service =>
                service.UpdatePageAsync(updatedPage: It.IsAny<Page>()))
            .ReturnsAsync(valueFunction: (Page page) => page);

        // When
        await pageProcessingService.UpdatePageAsync(updatedPage: updatedPage);

        // Then
        firstPage.Order.Should()
            .Be(expected: 1);

        secondPage.Order.Should()
            .Be(expected: 2);

        movedPage.Order.Should()
            .Be(expected: 0);

        pageServiceMock.Verify(expression: service =>
                service.UpdatePageAsync(updatedPage: It.IsAny<Page>()),
            times: Times.Exactly(callCount: 3));
    }

    [Fact]
    public async Task ShouldUpdatePageWhenUserCanUpdatePageForUpdateAsync()
    {

        // Given



        User actor = TestUsers.WithPrivilege(privilege: "app_admin", appId: 1);

        PageInfo pageInfo = new()
        {
            CultureId = string.Empty,
            Title = "Home",
            Description = "Home",
            Keywords = "Home",
        };

        Page dbPage = CreateRandomPage();
        Page page = CreateRandomPage();
        dbPage.AppId = 1;
        page.Id = dbPage.Id;
        page.AppId = dbPage.AppId;
        page.Name = dbPage.Name;
        page.Path = dbPage.Path;
        page.ParentId = dbPage.ParentId;
        page.PageInfo = [pageInfo];
        page.Contents = [];
        page.Roles = [];

        dbPage.PageInfo =
        [
            new PageInfo
            {
                CultureId = string.Empty,
                Title = "Home",
                Description = "Home",
                Keywords = "Home",
            },
        ];

        dbPage.Contents = [];
        dbPage.Roles = [];

        currentUser = actor;

        pageServiceMock.Setup(expression: x => x.GetAllPages(ignoreFilters: true))
            .Returns(value: new[] { dbPage }.AsQueryable());

        pageServiceMock.Setup(expression: x => x.GetAllPages())
            .Returns(value: new[] { dbPage }.AsQueryable());

        pageServiceMock.Setup(expression: x => x.UpdatePageAsync(updatedPage: It.IsAny<Page>()))
            .ReturnsAsync(value: dbPage);

        // When
        Page result = await pageProcessingService.UpdatePageAsync(updatedPage: page);

        // Then
        result.Should()
            .BeSameAs(expected: dbPage);

        pageServiceMock.Verify(expression: x => x.GetAllPages(ignoreFilters: true), times: Times.Once);

        pageServiceMock.Verify(expression: x => x.UpdatePageAsync(updatedPage: It.Is<Page>(match: updated =>
            updated.Id == page.Id &&
            updated.AppId == page.AppId &&
            updated.Name == page.Name)), times: Times.Once);

        VerifyNoOtherPageServiceCalls();
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenNewParentCannotBeResolvedForUpdateAsync()
    {

        // Given



        User actor = TestUsers.WithPrivilege(privilege: "app_admin", appId: 1);
        Page dbPage = CreateRandomPage();
        Page page = CreateRandomPage();
        dbPage.AppId = 1;
        int missingParentId = dbPage.Id + 1000;

        dbPage.PageInfo =
        [
            new PageInfo
            {
                CultureId = string.Empty,
                Title = "Home",
                Description = "Home",
                Keywords = "Home",
            },
        ];

        dbPage.Contents = [];
        dbPage.Roles = [];
        dbPage.AppId = 1;
        page.Id = dbPage.Id;
        page.AppId = dbPage.AppId;
        page.Name = dbPage.Name;
        page.Path = dbPage.Path;
        page.ParentId = missingParentId;

        page.PageInfo =
        [
            new PageInfo
            {
                CultureId = string.Empty,
                Title = "Home",
                Description = "Home",
                Keywords = "Home",
            },
        ];

        page.Contents = [];
        page.Roles = [];

        currentUser = actor;

        pageServiceMock.Setup(expression: x => x.GetAllPages(ignoreFilters: true))
            .Returns(value: new[] { dbPage }.AsQueryable());

        pageServiceMock.Setup(expression: x => x.GetAllPages())
            .Returns(value: new[] { dbPage }.AsQueryable());

        // When
        Func<Task> act = async () => await pageProcessingService.UpdatePageAsync(updatedPage: page);

        // Then
        await act.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        pageServiceMock.Verify(
            expression: x => x.GetAllPages(ignoreFilters: true),
            times: Times.Once);

        VerifyNoOtherPageServiceCalls();
    }

}