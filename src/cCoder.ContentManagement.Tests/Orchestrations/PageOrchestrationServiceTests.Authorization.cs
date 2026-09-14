// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.ContentManagement.Models;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class PageOrchestrationServiceTests
{
    [Fact]
    public async Task AddPageAsync_WhenAuthorizationIsDenied_DoesNotPersistAsync()
    {
        // Given
        Page page = CreateRandomPage();
        page.Layout = "Default";
        page.PageInfo = [new PageInfo { CultureId = string.Empty, Title = "Home" }];
        page.Contents = [];

        pageProcessingServiceMock
            .Setup(expression: service => service.LayoutExistsForApp(
                appId: page.AppId,
                layoutName: page.Layout))
            .Returns(value: true);

        DenyAuthorization();

        // When
        Func<Task> action = async () =>
            await orchestrationService.AddPageAsync(newPage: page);

        // Then
        await action.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        pageProcessingServiceMock.Verify(expression: service => service.LayoutExistsForApp(
            appId: page.AppId,
            layoutName: page.Layout), times: Times.Once);

        pageProcessingServiceMock.VerifyNoOtherCalls();
        pageEventProcessingServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdatePageAsync_WhenAuthorizationIsDenied_DoesNotPersistAsync()
    {
        // Given
        Page page = CreateRandomPage();
        page.Layout = "Default";

        pageProcessingServiceMock
            .Setup(expression: service => service.LayoutExistsForApp(
                appId: page.AppId,
                layoutName: page.Layout))
            .Returns(value: true);

        pageProcessingServiceMock
            .Setup(expression: service => service.GetAllPage(ignoreFilters: true))
            .Returns(value: new[] { page }.AsQueryable());

        DenyAuthorization();

        // When
        Func<Task> action = async () =>
            await orchestrationService.UpdatePageAsync(updatedPage: page);

        // Then
        await action.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        pageProcessingServiceMock.Verify(expression: service => service.LayoutExistsForApp(
            appId: page.AppId,
            layoutName: page.Layout), times: Times.Once);

        pageProcessingServiceMock.Verify(expression: service =>
            service.GetAllPage(ignoreFilters: true), times: Times.Once);

        pageProcessingServiceMock.VerifyNoOtherCalls();
        pageEventProcessingServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task DeletePageAsync_WhenAuthorizationIsDenied_DoesNotPersistAsync()
    {
        // Given
        Page page = CreateRandomPage();

        pageProcessingServiceMock
            .Setup(expression: service => service.GetPage(pageId: page.Id))
            .Returns(value: page);

        authorizationProcessingServiceMock
            .Setup(expression: service => service.UserCanPageAuthorizationContext(
                context: It.IsAny<AuthorizationContext>()))
            .Returns(value: false);

        // When
        Func<Task> action = async () =>
            await orchestrationService.DeleteAsync(pageId: page.Id);

        // Then
        await action.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        pageProcessingServiceMock.Verify(expression: service => service.GetPage(pageId: page.Id), times: Times.Once);
        pageProcessingServiceMock.VerifyNoOtherCalls();
        pageEventProcessingServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task RecomputeAllForAppAsync_WhenUserIsNotAppAdmin_DoesNotPersistAsync()
    {
        // Given
        const int appId = 42;

        authorizationProcessingServiceMock
            .Setup(expression: service => service.IsAdminOfAppAuthorizationContext(
                context: It.IsAny<AuthorizationContext>()))
            .Returns(value: false);

        // When
        Func<Task> action = async () =>
            await orchestrationService.RecomputeAllForAppAsync(appId: appId);

        // Then
        await action.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        pageProcessingServiceMock.VerifyNoOtherCalls();
        pageEventProcessingServiceMock.VerifyNoOtherCalls();
    }

    private void DenyAuthorization() =>
        authorizationProcessingServiceMock
            .Setup(expression: service => service.AuthorizeAuthorizationContext(
                context: It.IsAny<AuthorizationContext>()))
            .Throws(exception: new SecurityException(message: "Access Denied!"));
}