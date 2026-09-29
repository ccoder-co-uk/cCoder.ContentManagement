// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using Moq;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Events;

public partial class PageInfoEventServiceTests
{
    private readonly Mock<IPageInfoEventBroker> pageInfoEventBrokerMock;
    private readonly cCoder.ContentManagement.Services.Foundations.Events.PageInfoEventService service;
    private const string CurrentUserId = "test-user";

    public PageInfoEventServiceTests()
    {
        pageInfoEventBrokerMock = new Mock<IPageInfoEventBroker>(behavior: MockBehavior.Strict);
        pageInfoEventBrokerMock = new(behavior: MockBehavior.Strict);

        service = new cCoder.ContentManagement.Services.Foundations.Events.PageInfoEventService(
pageInfoEventBroker: pageInfoEventBrokerMock.Object
        );
    }
}