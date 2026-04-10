using cCoder.ContentManagement.Brokers.Events;
using cCoder.ContentManagement.Services.Foundations.Events;
using Moq;

namespace cCoder.Core.Services.Tests.CMS.Foundations.Events;

public partial class EventHandlerServiceTests
{
    private readonly Mock<IEventHubBroker> eventHubBrokerMock;
    private readonly EventHandlerService service;

    public EventHandlerServiceTests()
    {
        eventHubBrokerMock = new Mock<IEventHubBroker>(MockBehavior.Strict);
        service = new EventHandlerService(eventHubBrokerMock.Object);
    }
}
