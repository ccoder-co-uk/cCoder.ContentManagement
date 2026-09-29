// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using Moq;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Events;

public partial class ComponentEventServiceTests
{
    private readonly Mock<IComponentEventBroker> componentEventBrokerMock;
    private readonly cCoder.ContentManagement.Services.Foundations.Events.ComponentEventService service;
    private const string CurrentUserId = "test-user";

    public ComponentEventServiceTests()
    {
        componentEventBrokerMock = new Mock<IComponentEventBroker>(behavior: MockBehavior.Strict);
        componentEventBrokerMock = new(behavior: MockBehavior.Strict);

        service = new cCoder.ContentManagement.Services.Foundations.Events.ComponentEventService(
componentEventBroker: componentEventBrokerMock.Object
        );
    }
}