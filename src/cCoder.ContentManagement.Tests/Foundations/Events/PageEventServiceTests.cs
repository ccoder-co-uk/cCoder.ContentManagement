// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using Moq;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Events;

public partial class PageEventServiceTests
{
    private readonly Mock<IPageEventBroker> pageEventBrokerMock;
    private readonly cCoder.ContentManagement.Services.Foundations.Events.PageEventService service;
    private const string CurrentUserId = "test-user";

    public PageEventServiceTests()
    {
        pageEventBrokerMock = new Mock<IPageEventBroker>(behavior: MockBehavior.Strict);
        pageEventBrokerMock = new(behavior: MockBehavior.Strict);

        service = new cCoder.ContentManagement.Services.Foundations.Events.PageEventService(
pageEventBroker: pageEventBrokerMock.Object
        );
    }
}