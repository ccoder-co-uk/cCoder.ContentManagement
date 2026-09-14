// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.Security;
using ComponentRenderParams = cCoder.ContentManagement.Models.ComponentRenderParams;
using Config = cCoder.ContentManagement.Models.ContentManagementConfiguration;
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

        pageProcessingServiceMock
            .Setup(expression: service => service.LayoutExistsForApp(
                appId: entity.AppId,
                layoutName: entity.Layout))
            .Returns(value: true);

        pageProcessingServiceMock
            .Setup(expression: service => service.GetAllPage(ignoreFilters: true))
            .Returns(value: new[] { entity }.AsQueryable());

        pageProcessingServiceMock.Setup(expression: x => x.UpdatePageAsync(updatedPage: entity))
            .ReturnsAsync(value: entity);

        pageEventProcessingServiceMock
            .Setup(expression: x => x.RaisePageUpdateEventAsync(entity: entity, userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        Page result = await orchestrationService.UpdatePageAsync(updatedPage: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        pageProcessingServiceMock.Verify(expression: service => service.LayoutExistsForApp(
            appId: entity.AppId,
            layoutName: entity.Layout), times: Times.Once);

        pageProcessingServiceMock.Verify(expression: service =>
            service.GetAllPage(ignoreFilters: true), times: Times.Once);

        pageProcessingServiceMock.Verify(expression: x => x.UpdatePageAsync(updatedPage: entity), times: Times.Once);
        pageEventProcessingServiceMock.Verify(expression: x => x.RaisePageUpdateEventAsync(entity: entity, userId: CurrentUserId), times: Times.Once);
    }

    [Fact]
    public async Task ShouldThrowValidationExceptionWhenUpdateAsyncGivenUnknownLayout()
    {
        // Given
        Page entity = CreateRandomPage();
        entity.Layout = "MissingLayout";

        pageProcessingServiceMock
            .Setup(expression: service => service.LayoutExistsForApp(
                appId: entity.AppId,
                layoutName: entity.Layout))
            .Returns(value: false);

        // When
        Func<Task> act = async () => await orchestrationService.UpdatePageAsync(updatedPage: entity);

        // Then

        await act.Should()
            .ThrowAsync<ValidationException>()
            .WithMessage(expectedWildcardPattern: $"Layout '{entity.Layout}' does not exist for app {entity.AppId}.");

        pageProcessingServiceMock.Verify(expression: service => service.LayoutExistsForApp(
            appId: entity.AppId,
            layoutName: entity.Layout), times: Times.Once);

        pageProcessingServiceMock.VerifyNoOtherCalls();
        pageEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}