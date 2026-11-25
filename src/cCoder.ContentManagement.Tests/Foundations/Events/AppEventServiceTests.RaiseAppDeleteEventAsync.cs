// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.CMS;
using cCoder.Eventing.Models;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;
using CmsDataModels = cCoder.Data.Models.CMS;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Events;

public partial class AppEventServiceTests
{
    [Fact]
    public async Task ShouldPublishSerializableAppRowWhenDeletingAppWithTrackedRoles()
    {
        // Given
        App app = new() { Id = 109, TenantId = "tenant", Domain = "fixture.invalid" };
        app.Roles = [new() { App = app }];
        EventMessage<App> published = null;

        appEventBrokerMock
            .Setup(expression: broker => broker.RaiseAppDeleteEventAsync(message: It.IsAny<EventMessage<App>>()))
            .Callback<EventMessage<App>>(action: message => published = message)
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseAppDeleteEventAsync(app: app, userId: CurrentUserId);

        // Then
        string json = JsonConvert.SerializeObject(value: published);
        JObject payload = JObject.Parse(json: json);

        ((int)payload["Data"]["Id"]).Should()
            .Be(expected: app.Id);

        ((string)payload["Data"]["TenantId"]).Should()
            .Be(expected: app.TenantId);

        ((string)payload["Data"]["Domain"]).Should()
            .Be(expected: app.Domain);

        published.Data.Roles.Should()
            .BeNull();

        published.AuthInfo.SSOUserId.Should()
            .Be(expected: CurrentUserId);
    }

    [Fact]
    public async Task ShouldMapAndCallBrokerWhenRaiseAppDeleteEventAsync()
    {
        // Given
        App entity = new();
        EventMessage<CmsDataModels.App> actualMessage = null;

        appEventBrokerMock
            .Setup(expression: x => x.RaiseAppDeleteEventAsync(message: It.IsAny<EventMessage<CmsDataModels.App>>()))
            .Callback<EventMessage<CmsDataModels.App>>(action: message => actualMessage = message)
            .Returns(value: ValueTask.CompletedTask);

        // When
        await service.RaiseAppDeleteEventAsync(
            app: entity,
            userId: CurrentUserId);

        // Then

        actualMessage.Should()
            .NotBeNull();

        actualMessage!.Data.Should()
            .BeEquivalentTo(expectation: entity);

        actualMessage.AuthInfo.Should()
            .NotBeNull();

        actualMessage.AuthInfo.SSOUserId.Should()
            .Be(expected: CurrentUserId);

        appEventBrokerMock.Verify(
expression: x => x.RaiseAppDeleteEventAsync(message: It.IsAny<EventMessage<CmsDataModels.App>>()),
times: Times.Once
        );

        appEventBrokerMock.VerifyNoOtherCalls();
    }

}