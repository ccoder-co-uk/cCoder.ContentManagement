// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using Moq;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Events;

public partial class PageRoleEventServiceTests
{
    private readonly Mock<IPageRoleEventBroker> pageRoleEventBrokerMock;
    private readonly cCoder.ContentManagement.Services.Foundations.Events.PageRoleEventService service;
    private const string CurrentUserId = "test-user";

    public PageRoleEventServiceTests()
    {
        pageRoleEventBrokerMock = new Mock<IPageRoleEventBroker>(behavior: MockBehavior.Strict);
        pageRoleEventBrokerMock = new(behavior: MockBehavior.Strict);

        service = new cCoder.ContentManagement.Services.Foundations.Events.PageRoleEventService(
pageRoleEventBroker: pageRoleEventBrokerMock.Object
        );
    }
}