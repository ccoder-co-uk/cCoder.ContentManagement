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

public partial class PageInfoOrchestrationServiceTests
{
    [Fact]
    public async Task AddPageInfoAsync_WhenAuthorizationDenied_DoesNotCallPersistenceAsync()
    {
        // Given
        PageInfo pageInfo = CreateRandomPageInfo();
        pageInfo.PageId = 17;

        SetupOwningPage(pageId: pageInfo.PageId);

        authorizationProcessingServiceMock
            .Setup(expression: service => service.AuthorizeAuthorizationContext(
                context: It.Is<AuthorizationContext>(match: match =>
                    match.Request.AppId == 7
                    && match.Request.Privilege == "PageInfo_create")))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () =>
            await orchestrationService.AddPageInfoAsync(newPageInfo: pageInfo);

        // Then
        await action.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        VerifyOwningAppLookup(pageId: pageInfo.PageId);
        pageInfoProcessingServiceMock.VerifyNoOtherCalls();
        pageInfoEventProcessingServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdatePageInfoAsync_WhenAuthorizationDenied_DoesNotCallPersistenceAsync()
    {
        // Given
        PageInfo pageInfo = CreateRandomPageInfo();
        pageInfo.PageId = 17;
        SetupOwningPage(pageId: pageInfo.PageId);
        SetupDeniedAuthorization(privilege: "PageInfo_update");

        // When
        Func<Task> action = async () =>
            await orchestrationService.UpdatePageInfoAsync(
                updatedPageInfo: pageInfo);

        // Then
        await action.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        VerifyOwningAppLookup(pageId: pageInfo.PageId);
        pageInfoProcessingServiceMock.VerifyNoOtherCalls();
        pageInfoEventProcessingServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task DeletePageInfoAsync_WhenAuthorizationDenied_DoesNotCallPersistenceAsync()
    {
        // Given
        PageInfo pageInfo = CreateRandomPageInfo();
        pageInfo.Id = 9;
        pageInfo.PageId = 17;

        pageInfoProcessingServiceMock
            .Setup(expression: service => service.GetPageInfo(
                pageInfoId: pageInfo.Id))
            .Returns(value: pageInfo);

        SetupOwningPage(pageId: pageInfo.PageId);
        SetupDeniedAuthorization(privilege: "PageInfo_delete");

        // When
        Func<Task> action = async () =>
            await orchestrationService.DeleteAsync(pageInfoId: pageInfo.Id);

        // Then
        await action.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        pageInfoProcessingServiceMock.Verify(
            expression: service => service.GetPageInfo(
                pageInfoId: pageInfo.Id),
            times: Times.Once);

        VerifyOwningAppLookup(pageId: pageInfo.PageId);
        pageInfoProcessingServiceMock.VerifyNoOtherCalls();
        pageInfoEventProcessingServiceMock.VerifyNoOtherCalls();
    }

    private void SetupDeniedAuthorization(string privilege) =>
        authorizationProcessingServiceMock
            .Setup(expression: service => service.AuthorizeAuthorizationContext(
                context: It.Is<AuthorizationContext>(match: match =>
                    match.Request.AppId == 7
                    && match.Request.Privilege == privilege)))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

    private void SetupOwningPage(int pageId) =>
        pageInfoProcessingServiceMock
            .Setup(expression: service => service.GetOwningAppId(
                pageId: pageId))
            .Returns(value: 7);

    private void VerifyOwningAppLookup(int pageId) =>
        pageInfoProcessingServiceMock.Verify(
            expression: service => service.GetOwningAppId(
                pageId: pageId),
            times: Times.Once);
}