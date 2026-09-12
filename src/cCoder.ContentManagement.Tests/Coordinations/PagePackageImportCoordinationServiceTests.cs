// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Coordinations;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Coordinations;

public sealed partial class PagePackageImportCoordinationServiceTests
{
    [Fact]
    public async Task ImportPagesAsync_WhenPagesContainChildren_PersistsRootsBeforeChildrenAsync()
    {
        // Given
        const int appId = 17;

        Page sourcePage = new() { Name = "Home" };
        Page persistedPage = new() { Id = 29, AppId = appId, Name = "Home" };
        MockSequence sequence = new();
        Mock<IPageOrchestrationService> pageService = new(behavior: MockBehavior.Strict);
        Mock<IPageImportOrchestrationService> childService = new(behavior: MockBehavior.Strict);

        pageService.InSequence(sequence: sequence)
            .Setup(expression: service => service.ImportPagesAsync(
                appId: appId,
                items: It.Is<Page[]>(match: pages => pages.Single() == sourcePage)))
            .ReturnsAsync(value: [persistedPage]);

        childService.InSequence(sequence: sequence)
            .Setup(expression: service => service.HandlePageImportAsync(
                page: It.Is<Page>(match: page => page == persistedPage && page.Id == 29)))
            .Returns(value: ValueTask.CompletedTask);

        PagePackageImportCoordinationService service = new(
            pageOrchestrationService: pageService.Object,
            pageImportOrchestrationService: childService.Object);

        // When
        await service.ImportPagesAsync(appId: appId, pages: [sourcePage]);

        // Then
        pageService.VerifyAll();
        childService.VerifyAll();
    }
}