using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.Security;
using ComponentRenderParams = cCoder.ContentManagement.Models.ComponentRenderParams;
using Config = cCoder.ContentManagement.Models.Config;
using PageRenderParams = cCoder.ContentManagement.Models.PageRenderParams;
using PageRoleInfo = cCoder.ContentManagement.Models.PageRoleInfo;
using RenderParams = cCoder.ContentManagement.Models.RenderParams;
using RenderResult = cCoder.ContentManagement.Models.RenderResult;
using System.ComponentModel.DataAnnotations;
using TemplateRenderParams = cCoder.ContentManagement.Models.TemplateRenderParams;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class PageOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseUpdateEventAsyncWhenUpdateAsync()
    {
        // Given
        Page entity = CreateRandomPage();
        entity.Layout = "Default";
        layoutProcessingServiceMock
            .Setup(x => x.GetAll(true))
            .Returns(new[] { CreateLayoutFor(entity) }.AsQueryable());
        pageProcessingServiceMock.Setup(x => x.UpdateAsync(entity)).ReturnsAsync(entity);

        pageEventProcessingServiceMock
            .Setup(x => x.RaisePageUpdateEventAsync(entity))
            .Returns(ValueTask.CompletedTask);

        // When
        Page result = await orchestrationService.UpdateAsync(entity);

        // Then
        result.Should().BeSameAs(entity);
        layoutProcessingServiceMock.Verify(x => x.GetAll(true), Times.Once);
        pageProcessingServiceMock.Verify(x => x.UpdateAsync(entity), Times.Once);
        pageEventProcessingServiceMock.Verify(x => x.RaisePageUpdateEventAsync(entity), Times.Once);
    }

    [Fact]
    public async Task ShouldThrowValidationExceptionWhenUpdateAsyncGivenUnknownLayout()
    {
        // Given
        Page entity = CreateRandomPage();
        entity.Layout = "MissingLayout";
        layoutProcessingServiceMock
            .Setup(x => x.GetAll(true))
            .Returns(Array.Empty<Layout>().AsQueryable());

        // When
        Func<Task> act = async () => await orchestrationService.UpdateAsync(entity);

        // Then
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage($"Layout '{entity.Layout}' does not exist for app {entity.AppId}.");
        layoutProcessingServiceMock.Verify(x => x.GetAll(true), Times.Once);
        pageProcessingServiceMock.VerifyNoOtherCalls();
        pageEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}




















