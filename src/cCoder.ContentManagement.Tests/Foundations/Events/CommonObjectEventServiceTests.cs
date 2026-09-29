// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using Moq;

namespace cCoder.Core.Services.Tests.CMS.Foundations.Events;

public partial class CommonObjectEventServiceTests
{
    private readonly Mock<ICommonObjectEventBroker> commonObjectEventBrokerMock;
    private readonly cCoder.ContentManagement.Services.Foundations.Events.CommonObjectEventService service;
    private const string CurrentUserId = "test-user";

    public CommonObjectEventServiceTests()
    {
        commonObjectEventBrokerMock = new Mock<ICommonObjectEventBroker>(behavior: MockBehavior.Strict);
        commonObjectEventBrokerMock = new(behavior: MockBehavior.Strict);

        service = new cCoder.ContentManagement.Services.Foundations.Events.CommonObjectEventService(
commonObjectEventBroker: commonObjectEventBrokerMock.Object
        );
    }
}