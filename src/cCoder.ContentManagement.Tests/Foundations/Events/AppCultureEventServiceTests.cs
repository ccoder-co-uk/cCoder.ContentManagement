// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using Moq;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Events;

public partial class AppCultureEventServiceTests
{
    private readonly Mock<IAppCultureEventBroker> appCultureEventBrokerMock;
    private readonly cCoder.ContentManagement.Services.Foundations.Events.AppCultureEventService service;
    private const string CurrentUserId = "test-user";

    public AppCultureEventServiceTests()
    {
        appCultureEventBrokerMock = new Mock<IAppCultureEventBroker>(behavior: MockBehavior.Strict);
        appCultureEventBrokerMock = new(behavior: MockBehavior.Strict);

        service = new cCoder.ContentManagement.Services.Foundations.Events.AppCultureEventService(
appCultureEventBroker: appCultureEventBrokerMock.Object
        );
    }
}