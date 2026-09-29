// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class AppOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldAuthorizeThenCallProcessingWhenAddAsync()
    {
        // Given
        App entity = CreateRandomApp();
        List<string> calls = [];

        authorizationProcessingServiceMock
            .Setup(expression: service => service.AuthorizeAuthorizationContext(
                context: It.Is<AuthorizationContext>(match: context =>
                    context.Request.AppId == null
                    && context.Request.Privilege == "app_create")));

        appProcessingServiceMock
            .Setup(expression: service => service.GetAllApp(ignoreFilters: true))
            .Returns(value: Array.Empty<App>()
                .AsQueryable());

        appProcessingServiceMock
            .Setup(expression: service => service.PrepareNewApp(
                app: entity,
                isFirstApp: true))
            .Callback(action: () => calls.Add(item: "prepare"))
            .Returns(value: entity);

        appProcessingServiceMock
            .Setup(expression: x => x.AddAppAsync(newApp: entity))
            .Callback(action: () => calls.Add(item: "parent"))
            .ReturnsAsync(valueFunction: (App app) => app);

        appProcessingServiceMock
            .Setup(expression: service => service.StampAppChildren(app: entity))
            .Callback(action: () => calls.Add(item: "stamp"));

        appProcessingServiceMock
            .Setup(expression: service => service.PersistNewAppRolesAsync(app: entity))
            .Callback(action: () => calls.Add(item: "roles"))
            .Returns(value: ValueTask.CompletedTask);

        appEventProcessingServiceMock
            .Setup(expression: service => service.RaiseAppAddEventAsync(
                app: entity,
                userId: CurrentUserId))
            .Callback(action: () => calls.Add(item: "event"))
            .Returns(value: ValueTask.CompletedTask);

        // When
        App result = await orchestrationService.AddAppAsync(newApp: entity);

        // Then

        result.Should()
            .BeSameAs(expected: entity);

        calls.Should()
            .Equal(expected: ["prepare", "parent", "stamp", "roles", "event"]);

        appProcessingServiceMock.Verify(
            expression: x => x.AddAppAsync(newApp: It.IsAny<App>()),
            times: Times.Once);

        appProcessingServiceMock.VerifyAll();
        appEventProcessingServiceMock.VerifyAll();
    }
}