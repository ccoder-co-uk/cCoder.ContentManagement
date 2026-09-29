// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Events;
using Moq;


namespace cCoder.Core.Services.Tests.CMS.Foundations.Events;

public partial class SubmissionEventServiceTests
{
    private readonly Mock<ISubmissionEventBroker> submissionEventBrokerMock;
    private readonly cCoder.ContentManagement.Services.Foundations.Events.SubmissionEventService service;
    private const string CurrentUserId = "test-user";

    public SubmissionEventServiceTests()
    {
        submissionEventBrokerMock = new Mock<ISubmissionEventBroker>(behavior: MockBehavior.Strict);
        submissionEventBrokerMock = new(behavior: MockBehavior.Strict);

        service = new cCoder.ContentManagement.Services.Foundations.Events.SubmissionEventService(
submissionEventBroker: submissionEventBrokerMock.Object
        );
    }
}