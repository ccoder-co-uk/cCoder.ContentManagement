// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public sealed partial class PageImportOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldUpdateExistingContentWhenCultureCasingDiffersAsync()
    {
        // Given
        Content existing = new() { Id = 42, PageId = 7, Name = "body", CultureId = "en-GB", Html = "old" };
        Content incoming = new() { Name = "body", CultureId = "en-gb", Html = "new" };
        Page page = new() { Id = 7, Contents = [incoming] };
        Mock<IPageInfoProcessingService> pageInfo = new(MockBehavior.Strict);
        Mock<IContentProcessingService> content = new(MockBehavior.Strict);

        content
            .Setup(expression: service => service.GetAllContents(ignoreFilters: true))
            .Returns(value: new[] { existing }
                .AsQueryable());

        content
            .Setup(expression: service => service.UpdateContentAsync(updatedContent: incoming))
            .ReturnsAsync(value: incoming);

        PageImportOrchestrationService service = new(
            pageInfoProcessingService: pageInfo.Object,
            contentProcessingService: content.Object);

        // When
        await service.HandlePageImportAsync(page: page);

        // Then

        incoming.Id.Should()
            .Be(expected: existing.Id);

        incoming.PageId.Should()
            .Be(expected: page.Id);

        content.Verify(
            expression: service => service.UpdateContentAsync(updatedContent: incoming),
            times: Times.Once());

        content.Verify(
            expression: service => service.AddContentAsync(newContent: It.IsAny<Content>()),
            times: Times.Never());
    }

    [Fact]
    public async Task ShouldUpdateExistingPageInfoWhenCultureCasingDiffersAsync()
    {
        // Given
        PageInfo existing = new() { Id = 19, PageId = 7, CultureId = "fr-FR" };
        PageInfo incoming = new() { CultureId = "fr-fr" };
        Page page = new() { Id = 7, PageInfo = [incoming] };
        Mock<IPageInfoProcessingService> pageInfo = new(MockBehavior.Strict);
        Mock<IContentProcessingService> content = new(MockBehavior.Strict);

        pageInfo
            .Setup(expression: service => service.GetAllPageInfos(ignoreFilters: true))
            .Returns(value: new[] { existing }
                .AsQueryable());

        pageInfo
            .Setup(expression: service => service.UpdatePageInfoAsync(updatedPageInfo: incoming))
            .ReturnsAsync(value: incoming);

        PageImportOrchestrationService service = new(
            pageInfoProcessingService: pageInfo.Object,
            contentProcessingService: content.Object);

        // When
        await service.HandlePageImportAsync(page: page);

        // Then

        incoming.Id.Should()
            .Be(expected: existing.Id);

        incoming.PageId.Should()
            .Be(expected: page.Id);

        pageInfo.Verify(
            expression: service => service.UpdatePageInfoAsync(updatedPageInfo: incoming),
            times: Times.Once());

        pageInfo.Verify(
            expression: service => service.AddPageInfoAsync(newPageInfo: It.IsAny<PageInfo>()),
            times: Times.Never());
    }
}