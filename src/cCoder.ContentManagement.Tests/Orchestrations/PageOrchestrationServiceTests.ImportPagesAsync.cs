// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class PageOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldPersistParentWithoutRaisingEntityEventsAsync()
    {
        // Given
        const int appId = 17;
        const int persistedPageId = 29;
        Page page = CreateRandomPage();
        page.Id = 0;
        page.AppId = 0;
        page.Path = "Login";

        pageProcessingServiceMock
            .Setup(expression: service => service.GetAllPage(ignoreFilters: true))
            .Returns(value: Array.Empty<Page>().AsQueryable());

        pageProcessingServiceMock
            .Setup(expression: service => service.AddOrUpdatePageResult(
                newPage: It.Is<IEnumerable<Page>>(match: pages => pages.Single() == page)))
            .ReturnsAsync(value: [new OperationResult<Page>
            {
                Success = true,
                Item = page
            }])
            .Callback(callback: () => page.Id = persistedPageId);

        // When
        Page[] importedPages = await orchestrationService.ImportPagesAsync(
            appId: appId,
            pages: [page]);

        // Then
        page.Id.Should().Be(expected: persistedPageId);
        page.AppId.Should().Be(expected: appId);
        importedPages.Should().ContainSingle().Which.Should().BeSameAs(expected: page);
        pageProcessingServiceMock.VerifyAll();
        pageEventProcessingServiceMock.VerifyNoOtherCalls();
        layoutProcessingServiceMock.VerifyNoOtherCalls();
    }
}