// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Services.Foundations.Events;
using cCoder.ContentManagement.Services.Processings;
using FizzWare.NBuilder;
using Moq;


namespace cCoder.Core.Services.Tests.CMS.Processings;

public partial class SubmissionEventProcessingServiceTests
{
    private readonly Mock<ISubmissionEventService> submissionEventServiceMock;
    private readonly SubmissionEventProcessingService service;
    private const string CurrentUserId = "test-user";

    public SubmissionEventProcessingServiceTests()
    {
        submissionEventServiceMock = new Mock<ISubmissionEventService>(behavior: MockBehavior.Strict);
        service = new SubmissionEventProcessingService(
            eventService: submissionEventServiceMock.Object);
    }

    private static Submission CreateRandomSubmission() =>
        Builder<Submission>.CreateNew()
        .Build();
}