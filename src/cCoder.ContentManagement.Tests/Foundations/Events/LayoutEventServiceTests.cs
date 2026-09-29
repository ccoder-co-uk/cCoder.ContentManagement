// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using Moq;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Events;

public partial class LayoutEventServiceTests
{
    private readonly Mock<ILayoutEventBroker> layoutEventBrokerMock;
    private readonly cCoder.ContentManagement.Services.Foundations.Events.LayoutEventService service;
    private const string CurrentUserId = "test-user";

    public LayoutEventServiceTests()
    {
        layoutEventBrokerMock = new Mock<ILayoutEventBroker>(behavior: MockBehavior.Strict);
        layoutEventBrokerMock = new(behavior: MockBehavior.Strict);

        service = new cCoder.ContentManagement.Services.Foundations.Events.LayoutEventService(
layoutEventBroker: layoutEventBrokerMock.Object
        );
    }
}