// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using Moq;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Events;

public partial class ContentEventServiceTests
{
    private readonly Mock<IContentEventBroker> contentEventBrokerMock;
    private readonly cCoder.ContentManagement.Services.Foundations.Events.ContentEventService service;
    private const string CurrentUserId = "test-user";

    public ContentEventServiceTests()
    {
        contentEventBrokerMock = new Mock<IContentEventBroker>(behavior: MockBehavior.Strict);
        contentEventBrokerMock = new(behavior: MockBehavior.Strict);

        service = new cCoder.ContentManagement.Services.Foundations.Events.ContentEventService(
contentEventBroker: contentEventBrokerMock.Object
        );
    }
}