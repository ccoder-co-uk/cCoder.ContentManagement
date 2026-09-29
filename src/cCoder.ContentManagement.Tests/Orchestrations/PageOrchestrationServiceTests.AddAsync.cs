// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using Moq;
using Xunit;

#pragma warning disable STXFORMAT008, STXFORMAT009

namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class PageOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldCallProcessingThenRaiseAddEventAsyncWhenAddAsync()
    {
        // Given
        Page entity = CreateRandomPage();
        entity.Layout = "Default";
        entity.PageInfo = [new PageInfo { CultureId = string.Empty, Title = "Home" }];
        entity.Contents = [];

        pageProcessingServiceMock
            .Setup(expression: service => service.LayoutExistsForApp(
                appId: entity.AppId,
                layoutName: entity.Layout))
            .Returns(value: true);

        pageProcessingServiceMock
            .Setup(expression: x => x.AddPageAsync(newPage: entity))
            .ReturnsAsync(value: entity);

        pageEventProcessingServiceMock
            .Setup(expression: x => x.RaisePageAddEventAsync(entity: entity, userId: CurrentUserId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        Page result = await orchestrationService.AddPageAsync(newPage: entity);

        // Then
        Assert.Same(expected: entity, actual: result);

        pageProcessingServiceMock.Verify(expression: service => service.LayoutExistsForApp(
            appId: entity.AppId,
            layoutName: entity.Layout), times: Times.Once);
        pageProcessingServiceMock.Verify(expression: x => x.AddPageAsync(newPage: entity), times: Times.Once);
        pageEventProcessingServiceMock.Verify(expression: x => x.RaisePageAddEventAsync(entity: entity, userId: CurrentUserId), times: Times.Once);
    }

    [Fact]
    public async Task ShouldThrowValidationExceptionWhenAddAsyncGivenUnknownLayout()
    {
        // Given
        Page entity = CreateRandomPage();
        entity.Layout = "MissingLayout";
        entity.PageInfo = [new PageInfo { CultureId = string.Empty, Title = "Home" }];
        entity.Contents = [];

        pageProcessingServiceMock
            .Setup(expression: service => service.LayoutExistsForApp(
                appId: entity.AppId,
                layoutName: entity.Layout))
            .Returns(value: false);

        // When
        Func<Task> act = async () => await orchestrationService.AddPageAsync(newPage: entity);

        // Then
        await act
            .Should()
            .ThrowAsync<ValidationException>()
            .WithMessage(expectedWildcardPattern: $"Layout '{entity.Layout}' does not exist for app {entity.AppId}.");

        pageProcessingServiceMock.Verify(expression: service => service.LayoutExistsForApp(
            appId: entity.AppId,
            layoutName: entity.Layout), times: Times.Once);
        pageProcessingServiceMock.VerifyNoOtherCalls();
        pageEventProcessingServiceMock.VerifyNoOtherCalls();
    }
}