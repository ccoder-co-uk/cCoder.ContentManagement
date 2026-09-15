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

public partial class CultureOrchestrationServiceTests
{
    [Fact]
    public async Task AddCultureAsync_WhenAuthorizationDenied_DoesNotCallPersistenceAsync()
    {
        // Given
        Culture culture = CreateRandomCulture();

        SetupOwningApp(cultureId: culture.Id);

        authorizationProcessingServiceMock
            .Setup(expression: service => service.AuthorizeAuthorizationContext(
                context: It.Is<AuthorizationContext>(match: match =>
                    match.Request.AppId == 7
                    && match.Request.Privilege == "Culture_create")))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        // When
        Func<Task> action = async () =>
            await orchestrationService.AddCultureAsync(newCulture: culture);

        // Then
        await action.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        VerifyOwningAppLookup(cultureId: culture.Id);
        cultureProcessingServiceMock.VerifyNoOtherCalls();
        cultureEventProcessingServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdateCultureAsync_WhenAuthorizationDenied_DoesNotCallPersistenceAsync()
    {
        // Given
        Culture culture = CreateRandomCulture();
        SetupOwningApp(cultureId: culture.Id);
        SetupDeniedAuthorization(privilege: "Culture_update");

        // When
        Func<Task> action = async () =>
            await orchestrationService.UpdateCultureAsync(updatedCulture: culture);

        // Then
        await action.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        VerifyOwningAppLookup(cultureId: culture.Id);
        cultureProcessingServiceMock.VerifyNoOtherCalls();
        cultureEventProcessingServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task DeleteCultureAsync_WhenAuthorizationDenied_DoesNotCallPersistenceAsync()
    {
        // Given
        Culture culture = CreateRandomCulture();

        cultureProcessingServiceMock
            .Setup(expression: service => service.GetCulture(
                cultureId: culture.Id))
            .Returns(value: culture);

        SetupOwningApp(cultureId: culture.Id);
        SetupDeniedAuthorization(privilege: "Culture_delete");

        // When
        Func<Task> action = async () =>
            await orchestrationService.DeleteAsync(cultureId: culture.Id);

        // Then
        await action.Should()
            .ThrowAsync<SecurityException>()
            .WithMessage(expectedWildcardPattern: "Access Denied!");

        cultureProcessingServiceMock.Verify(
            expression: service => service.GetCulture(
                cultureId: culture.Id),
            times: Times.Once);

        VerifyOwningAppLookup(cultureId: culture.Id);
        cultureProcessingServiceMock.VerifyNoOtherCalls();
        cultureEventProcessingServiceMock.VerifyNoOtherCalls();
    }

    private void SetupDeniedAuthorization(string privilege) =>
        authorizationProcessingServiceMock
            .Setup(expression: service => service.AuthorizeAuthorizationContext(
                context: It.Is<AuthorizationContext>(match: match =>
                    match.Request.AppId == 7
                    && match.Request.Privilege == privilege)))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

    private void SetupOwningApp(string cultureId) =>
        cultureProcessingServiceMock
            .Setup(expression: service => service.GetOwningAppId(
                cultureId: cultureId))
            .Returns(value: 7);

    private void VerifyOwningAppLookup(string cultureId) =>
        cultureProcessingServiceMock.Verify(
            expression: service => service.GetOwningAppId(
                cultureId: cultureId),
            times: Times.Once);
}