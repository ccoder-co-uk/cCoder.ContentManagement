// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using Moq;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Events;

public partial class AppEventServiceTests
{
    private readonly Mock<IAppEventBroker> appEventBrokerMock;
    private readonly cCoder.ContentManagement.Services.Foundations.Events.AppEventService service;
    private const string CurrentUserId = "test-user";

    public AppEventServiceTests()
    {
        appEventBrokerMock = new Mock<IAppEventBroker>(behavior: MockBehavior.Strict);
        appEventBrokerMock = new(behavior: MockBehavior.Strict);

        service = new cCoder.ContentManagement.Services.Foundations.Events.AppEventService(
appEventBroker: appEventBrokerMock.Object
        );
    }
}