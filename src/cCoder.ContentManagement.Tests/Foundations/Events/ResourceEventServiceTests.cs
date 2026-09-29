// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using Moq;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Events;

public partial class ResourceEventServiceTests
{
    private readonly Mock<IResourceEventBroker> resourceEventBrokerMock;
    private readonly cCoder.ContentManagement.Services.Foundations.Events.ResourceEventService service;
    private const string CurrentUserId = "test-user";

    public ResourceEventServiceTests()
    {
        resourceEventBrokerMock = new Mock<IResourceEventBroker>(behavior: MockBehavior.Strict);
        resourceEventBrokerMock = new(behavior: MockBehavior.Strict);

        service = new cCoder.ContentManagement.Services.Foundations.Events.ResourceEventService(
resourceEventBroker: resourceEventBrokerMock.Object
        );
    }
}