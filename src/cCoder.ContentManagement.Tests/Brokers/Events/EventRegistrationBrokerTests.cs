// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.Data.Models.CMS;
using cCoder.Eventing;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.ContentManagement.Tests.Brokers.Events;

public sealed partial class EventRegistrationBrokerTests
{
    [Fact]
    public void ListenToEvent_WhenCalled_ShouldRegisterUnchangedEventBoundary()
    {
        // Given
        const string eventName = "app_delete";
        Mock<IEventHub> eventHubMock = new();
        EventRegistrationBroker broker = new(eventHub: eventHubMock.Object);

        Func<IAppOrchestrationService, App, ValueTask> handler =
            (service, app) => service.HandleAppDeleteAsync(app: app);

        // When
        broker.ListenToEvent(name: eventName, handler: handler);

        // Then
        eventHubMock.Invocations
            .Should()
            .ContainSingle();

        eventHubMock.Invocations[0].Arguments[0]
            .Should()
            .Be(expected: eventName);

        eventHubMock.Invocations[0].Arguments[1]
            .Should()
            .BeSameAs(expected: handler);
    }
}