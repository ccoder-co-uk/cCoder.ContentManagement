// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using Moq;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Events;

public partial class CultureEventServiceTests
{
    private readonly Mock<ICultureEventBroker> cultureEventBrokerMock;
    private readonly cCoder.ContentManagement.Services.Foundations.Events.CultureEventService service;
    private const string CurrentUserId = "test-user";

    public CultureEventServiceTests()
    {
        cultureEventBrokerMock = new Mock<ICultureEventBroker>(behavior: MockBehavior.Strict);
        cultureEventBrokerMock = new(behavior: MockBehavior.Strict);

        service = new cCoder.ContentManagement.Services.Foundations.Events.CultureEventService(
cultureEventBroker: cultureEventBrokerMock.Object
        );
    }
}