// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using cCoder.ContentManagement.Services.Foundations.Events;
using Moq;

namespace cCoder.Core.Services.Tests.CMS.Foundations.Events;

public partial class EventHandlerServiceTests
{
    private readonly Mock<IEventHubBroker> eventHubBrokerMock;
    private readonly EventHandlerService service;

    public EventHandlerServiceTests()
    {
        eventHubBrokerMock = new Mock<IEventHubBroker>(
            behavior: MockBehavior.Strict);
        service = new EventHandlerService(eventHubBroker: eventHubBrokerMock.Object);
    }
}