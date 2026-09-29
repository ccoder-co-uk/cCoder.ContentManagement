// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Services.Orchestrations;
using cCoder.ContentManagement.Services.Processings;
using FizzWare.NBuilder;
using Moq;
using cCoder.ContentManagement.Models;


namespace cCoder.Core.Services.Tests.CMS.Orchestrations;

public partial class SubmissionOrchestrationServiceTests
{
    private readonly Mock<ISubmissionProcessingService> submissionProcessingServiceMock;
    private readonly Mock<ISubmissionEventProcessingService> submissionEventProcessingServiceMock;
    private readonly Mock<IAuthorizationProcessingService> authorizationProcessingServiceMock;
    private readonly SubmissionOrchestrationService orchestrationService;
    private const string CurrentUserId = "test-user";

    public SubmissionOrchestrationServiceTests()
    {
        submissionProcessingServiceMock = new Mock<ISubmissionProcessingService>(behavior: MockBehavior.Strict);
        submissionEventProcessingServiceMock = new Mock<ISubmissionEventProcessingService>(behavior: MockBehavior.Strict);
        authorizationProcessingServiceMock = new(behavior: MockBehavior.Strict);
        authorizationProcessingServiceMock
            .Setup(expression: service => service.GetCurrentUserId())
            .Returns(value: CurrentUserId);

        authorizationProcessingServiceMock
            .Setup(expression: service => service.AuthorizeAuthorizationContext(
context:                 It.IsAny<AuthorizationContext>()));

        orchestrationService = new SubmissionOrchestrationService(
processingService: submissionProcessingServiceMock.Object,
eventService: submissionEventProcessingServiceMock.Object,
authorizationProcessingService: authorizationProcessingServiceMock.Object
        );
    }

    private static Submission CreateRandomSubmission() =>
        Builder<Submission>.CreateNew()
        .Build();
}