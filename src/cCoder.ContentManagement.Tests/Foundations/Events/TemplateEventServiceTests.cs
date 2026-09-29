// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using Moq;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Events;

public partial class TemplateEventServiceTests
{
    private readonly Mock<ITemplateEventBroker> templateEventBrokerMock;
    private readonly cCoder.ContentManagement.Services.Foundations.Events.TemplateEventService service;
    private const string CurrentUserId = "test-user";

    public TemplateEventServiceTests()
    {
        templateEventBrokerMock = new Mock<ITemplateEventBroker>(behavior: MockBehavior.Strict);
        templateEventBrokerMock = new(behavior: MockBehavior.Strict);

        service = new cCoder.ContentManagement.Services.Foundations.Events.TemplateEventService(
templateEventBroker: templateEventBrokerMock.Object
        );
    }
}