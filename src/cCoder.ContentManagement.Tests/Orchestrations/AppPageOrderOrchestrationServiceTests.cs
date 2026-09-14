// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Orchestrations;

public sealed class AppPageOrderOrchestrationServiceTests
{
    [Fact]
    public async Task UpdatePageOrder_WhenAuthorized_UpdatesMatchingPages()
    {
        // Given
        Page existingPage = new() { Id = 5, AppId = 42 };
        App app = new()
        {
            Id = 42,
            Pages = [new Page { Id = 5, Order = 7, ParentId = 3 }]
        };

        Mock<IPageProcessingService> pageService = new(MockBehavior.Strict);
        Mock<IAuthorizationProcessingService> authorizationService = new(MockBehavior.Strict);

        authorizationService.Setup(expression: service =>
            service.AuthorizeAuthorizationContext(
                It.Is<AuthorizationContext>(context =>
                    context.Request.AppId == 42
                    && context.Request.Privilege == "app_update")));

        pageService.Setup(expression: service => service.GetAllPage(true))
            .Returns(value: new[] { existingPage }.AsQueryable());

        pageService.Setup(expression: service => service.UpdatePageAsync(existingPage))
            .ReturnsAsync(value: existingPage);

        AppPageOrderOrchestrationService service = new(
            pageProcessingService: pageService.Object,
            authorizationProcessingService: authorizationService.Object);

        // When
        await service.UpdatePageOrderAppAsync(appId: 42, updatedApp: app);

        // Then
        existingPage.Order.Should().Be(expected: 7);
        existingPage.ParentId.Should().Be(expected: 3);
        authorizationService.VerifyAll();
        pageService.VerifyAll();
    }

    [Fact]
    public async Task UpdatePageOrder_WhenAuthorizationIsDenied_DoesNotReadOrPersistPages()
    {
        // Given
        Mock<IPageProcessingService> pageService = new(MockBehavior.Strict);
        Mock<IAuthorizationProcessingService> authorizationService = new(MockBehavior.Strict);

        authorizationService.Setup(expression: service =>
            service.AuthorizeAuthorizationContext(It.IsAny<AuthorizationContext>()))
            .Throws(exception: new SecurityException(message: "Access Denied!"));

        AppPageOrderOrchestrationService service = new(
            pageProcessingService: pageService.Object,
            authorizationProcessingService: authorizationService.Object);

        // When
        Func<Task> action = async () =>
            await service.UpdatePageOrderAppAsync(
                appId: 42,
                updatedApp: new App { Id = 42, Pages = [] });

        // Then
        await action.Should().ThrowAsync<SecurityException>();
        pageService.VerifyNoOtherCalls();
    }
}